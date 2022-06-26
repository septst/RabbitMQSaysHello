using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static System.Console;

var exchangeName = "direct_logs";
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchangeName,
    ExchangeType.Direct
);

var queueName = string.Empty; // channel.QueueDeclare().QueueName;

if (args.Length < 1)
{
    Error.WriteLine("Usage: {0} [info] [warning] [error]",
        Environment.GetCommandLineArgs()[0]);
    WriteLine(" Press [enter] to exit.");
    ReadLine();
    Environment.ExitCode = 1;
    return;
}

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    WriteLine(" [x] Received '{0}':'{1}'",
        routingKey, message);

    channel.BasicAck(
        ea.DeliveryTag,
        false);

    WriteLine(" Press any key to exit.");
};

foreach (var severity in args)
{
    queueName = channel.QueueDeclare().QueueName;

    channel.QueueBind(
        queueName,
        exchangeName,
        severity
    );

    channel.BasicConsume(
        queueName,
        false,
        consumer);
}

WriteLine(" [*] Waiting for messages.");
WriteLine(" Press any key to exit.");
ReadKey();