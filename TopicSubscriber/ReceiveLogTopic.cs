using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static System.Console;

const string exchangeName = "topic_logs";
var factory = new ConnectionFactory
{
    HostName = "localhost"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchangeName,
    "topic"
);

if (args.Length < 1)
{
    Error.WriteLine("Usage: {0} [binding_key...]",
        Environment.GetCommandLineArgs()[0]);
    WriteLine(" Press [enter] to exit.");
    ReadLine();
    Environment.ExitCode = 1;
    return;
}

var queueName = string.Empty; // channel.QueueDeclare().QueueName;

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    var deliveryTag = ea.DeliveryTag;

    WriteLine(" [x] Received '{0}':'{1}' with delivery tag {2}",
        routingKey,
        message,
        deliveryTag);

    channel.BasicAck(
        deliveryTag,
        false);

    WriteLine(" [*] Waiting for messages. Press any key to exit.");
};


foreach (var bindingKey in args)
{
    queueName = channel.QueueDeclare().QueueName;

    channel.QueueBind(
        queueName,
        exchangeName,
        bindingKey);

    channel.BasicConsume(
        queueName,
        false,
        consumer);
}

WriteLine(" [*] Waiting for messages. Press any key to exit.");
ReadKey();