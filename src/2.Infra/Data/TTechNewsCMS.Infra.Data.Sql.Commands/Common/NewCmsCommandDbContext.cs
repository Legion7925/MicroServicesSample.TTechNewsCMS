using Microsoft.EntityFrameworkCore;
using TTechNewsCMS.Core.Domain.NewsAgg.Entities;
using Zamin.Core.Domain.Toolkits.ValueObjects;
using Zamin.Core.Domain.ValueObjects;
using Zamin.Extensions.Events.Outbox.Dal.EF;
using Zamin.Infra.Data.Sql.Commands;
using Zamin.Infra.Data.Sql.Commands.ValueConversions;

namespace NewCms.Infra.Data.Sql.Commands.Common
{
    public class NewCmsCommandDbContext : BaseOutboxCommandDbContext
    {
        public DbSet<News> News { get; set; }
        public NewCmsCommandDbContext(DbContextOptions<NewCmsCommandDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(builder);
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {

            configurationBuilder.Properties<Description>().HaveConversion<DescriptionConversion>();
            configurationBuilder.Properties<Title>().HaveConversion<TitleConversion>();
            configurationBuilder.Properties<BusinessId>().HaveConversion<BusinessIdConversion>();
        }
    }
}
