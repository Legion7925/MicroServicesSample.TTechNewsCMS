
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.SqlClient;
using System.Text;
using Zamin.Core.Domain.Events;

namespace TTechNewsCMS.Endpoints.API.BackgroundTask;

/// <summary>
/// these background tasks should go in a different projects in real world scenario
/// </summary>
public class KeywordCreatedReceiver : BackgroundService
{
    private readonly IConnection connection;
    private readonly IModel model;
    private readonly string routingKey = "TTechNewsExchange.BasicInfo.Event.KeywordCreated";
    private readonly string exchangeName = "TTechNewsExchange";
    private string queueName;
    private int messageCount;
    private EventingBasicConsumer consumer;
    private readonly SqlConnection sqlConnection = new SqlConnection("Server =.\\SQLEXPRESS; Database=TTechNewsDb; Integrated Security=True;MultipleActiveResultSets=true;TrustServerCertificate=True;");

    public KeywordCreatedReceiver()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
        };

        connection = factory.CreateConnection();
        model = connection.CreateModel();
        model.ExchangeDeclare(exchangeName, ExchangeType.Topic, false, false, null);
        queueName = model.QueueDeclare().QueueName;
        model.QueueBind(queueName, exchangeName, routingKey, null);
        consumer = new(model);
        consumer.Received += Consumer_Received;
        model.BasicConsume(queueName, true, consumer);
        Console.WriteLine($"Start receiving keyword events");
        sqlConnection.Open();

    }

    private void Consumer_Received(object? sender, BasicDeliverEventArgs e)
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        var keywordEvent = JsonConvert.DeserializeObject<KeywordCreated>(message);
        string command = $"INSERT INTO [dbo].[Keyword]([KeywordBusinessId],[KeywordTitle])VALUES('{keywordEvent.BusinessId}',N'{keywordEvent.Title}')";
        SqlCommand sqlCommand = new(command, sqlConnection);
        sqlCommand.ExecuteNonQuery();
        messageCount++;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is not true)
        {
            Console.WriteLine($"Message Count is {messageCount} at {DateTime.Now}");

            await Task.Delay(10000, stoppingToken);

        }
    }
}

public class KeywordCreated
{
    public Guid BusinessId { get; set; }

    public string Title { get; set; }
}

