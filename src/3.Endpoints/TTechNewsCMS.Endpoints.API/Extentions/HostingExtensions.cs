using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NewCms.Infra.Data.Sql.Commands.Common;
using NewCms.Infra.Data.Sql.Queries.Common;
using Serilog;
using Steeltoe.Discovery.Client;
using TTechNewsCMS.Endpoints.API.BackgroundTask;
using Zamin.EndPoints.Web.Extensions.ModelBinding;
using Zamin.Extensions.DependencyInjection;
using Zamin.Extensions.Events.Outbox.Dal.EF.Interceptors;
using Zamin.Infra.Data.Sql.Commands.Interceptors;

namespace TTechNewsCMS.Endpoints.API.Extentions;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        IConfiguration configuration = builder.Configuration;
        builder.Services.AddDiscoveryClient();
        builder.Services.AddZaminParrotTranslator(c =>
        {
            c.ConnectionString = configuration.GetConnectionString("CommandDb_ConnectionString");
            c.AutoCreateSqlTable = true;
            c.SchemaName = "dbo";
            c.TableName = "ParrotTranslations";
            c.ReloadDataIntervalInMinuts = 1;
        });

        builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", c =>
        {
            c.Authority = "https://localhost:5001/";
            c.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateAudience = false,
            };
        });

        builder.Services.AddAuthorization(c =>
        {
            c.AddPolicy("NewsCmsPolicy", p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireClaim("scope", "newscms");
            });
        });

        builder.Services.AddZaminWebUserInfoService(c =>
        {
            c.DefaultUserId = "1";
        }, true);

        builder.Services.AddZaminAutoMapperProfiles(option =>
        {
            option.AssmblyNamesForLoadProfiles = "Miniblog";
        });

        builder.Services.AddZaminMicrosoftSerializer();

        builder.Services.AddZaminInMemoryCaching();


        builder.Services.AddDbContext<NewCmsCommandDbContext>(c => c.UseSqlServer(configuration.GetConnectionString("CommandDb_ConnectionString")).AddInterceptors(new SetPersianYeKeInterceptor(), new AddOutBoxEventItemInterceptor(), new AddAuditDataInterceptor()));

        builder.Services.AddDbContext<NewCmsQueryDbContext>(c => c.UseSqlServer(configuration.GetConnectionString("QueryDb_ConnectionString")));

        //PollingPublisher
        //builder.Services.AddZaminPollingPublisherDalSql(configuration, "PollingPublisherSqlStore");
        //builder.Services.AddZaminPollingPublisher(configuration, "PollingPublisher");

        //MessageInbox
        //builder.Services.AddZaminMessageInboxDalSql(configuration, "MessageInboxSqlStore");
        //builder.Services.AddZaminMessageInbox(configuration, "MessageInbox");

        //builder.Services.AddZaminRabbitMqMessageBus(configuration, "RabbitMq");

        //builder.Services.AddZaminTraceJeager(configuration, "OpenTeletmetry");


        builder.Services.AddZaminApiCore("Zamin", "MiniBlog");

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHostedService<KeywordCreatedReceiver>();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks().AddDbContextCheck<NewCmsCommandDbContext>();
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        //zamin
        app.UseZaminApiExceptionHandler();

        //Serilog
        app.UseSerilogRequestLogging();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStatusCodePages();

        app.UseCors(delegate (CorsPolicyBuilder builder)
        {
            builder.AllowAnyOrigin();
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
        });

        //app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        var controllerBuilder = app.MapControllers().RequireAuthorization("NewsCmsPolicy");

        app.MapHealthChecks("/health/live" , new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => false
        });
        app.MapHealthChecks("/health/ready");

        return app;
    }
}