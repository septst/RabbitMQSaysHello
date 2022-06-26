using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    "logs",
    ExchangeType.Fanout
);

var queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(
    queueName,
    "logs",
    ""
);

Console.WriteLine(" [*] Waiting for logs.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [x] Received {0}", message);

    channel.BasicAck(
        ea.DeliveryTag,
        false);

    Console.WriteLine(" [*] Waiting for messages.");
    Console.WriteLine("Press any key to exit.");
};

channel.BasicConsume(queueName,
    false,
    consumer);

Console.WriteLine(" Press any key to exit.");
Console.ReadLine();