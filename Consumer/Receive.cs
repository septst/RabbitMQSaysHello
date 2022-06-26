using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare("hello",
    false,
    false,
    false,
    null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[x] consumed {message} message.");

    channel.BasicAck(
        eventArgs.DeliveryTag,
        false);

    Console.WriteLine(" [*] Waiting for messages.");
    Console.WriteLine("Press any key to exit.");
};

Console.WriteLine(" [*] Waiting for messages.");

channel.BasicConsume(
    "hello",
    false,
    consumer
);

Console.WriteLine("Press any key to exit.");
Console.ReadKey();