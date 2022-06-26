using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static System.Console;

const string exchangeName = "logs";
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchangeName,
    ExchangeType.Fanout
);

var queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(
    queueName,
    exchangeName,
    ""
);

WriteLine(" [*] Waiting for logs.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    WriteLine(" [x] Received {0}", message);

    channel.BasicAck(
        ea.DeliveryTag,
        false);

    WriteLine(" [*] Waiting for messages.");
    WriteLine("Press any key to exit.");
};

channel.BasicConsume(queueName,
    false,
    consumer);

WriteLine(" Press any key to exit.");
ReadLine();