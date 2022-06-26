using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static System.Console;

const string queueName = "task_queue";
var factory = new ConnectionFactory
{
    HostName = "localhost"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queueName,
    true,
    false,
    false,
    null
);

channel.BasicQos(
    0,
    1,
    false);

WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model,
    eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    WriteLine($"[x] consumed {message} message.");

    var dots = message.Split('.')
        .Length - 1;
    Thread.Sleep(dots * 1000);

    WriteLine(" [x] Done");

    channel.BasicAck(
        eventArgs.DeliveryTag,
        false);

    WriteLine(" [*] Waiting for messages.");
    WriteLine("Press any key to exit.");
};

channel.BasicConsume(
    queueName,
    false,
    consumer
);

WriteLine("Press any key to exit.");
ReadKey();