using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
        Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    Console.WriteLine(" [x] Received '{0}':'{1}'",
        routingKey, message);

    channel.BasicAck(
        ea.DeliveryTag,
        false);

    Console.WriteLine(" Press any key to exit.");
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

Console.WriteLine(" [*] Waiting for messages.");
Console.WriteLine(" Press any key to exit.");
Console.ReadKey();