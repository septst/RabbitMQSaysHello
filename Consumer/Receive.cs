using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static System.Console;

const string queueName = "hello";
var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queueName,
    false,
    false,
    false,
    null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (_, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    WriteLine($"[x] consumed {message} message with sequence number {eventArgs.DeliveryTag}.");

    channel.BasicAck(
        eventArgs.DeliveryTag,
        false);

    WriteLine(" [*] Waiting for messages.");
    WriteLine("Press any key to exit.");
};

WriteLine(" [*] Waiting for messages.");

channel.BasicConsume(
    queueName,
    false,
    consumer
);

WriteLine("Press any key to exit.");
ReadKey();