using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    HostName = "localhost"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    "task_queue",
    true,
    false,
    false,
    null
);

channel.BasicQos(
    prefetchSize: 0,
    prefetchCount: 1,
    global: false);

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model,
    eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[x] consumed {message} message.");

    var dots = message.Split('.')
                   .Length - 1;
    Thread.Sleep(dots * 1000);

    Console.WriteLine(" [x] Done");

    channel.BasicAck(
        eventArgs.DeliveryTag,
        false);
    
    Console.WriteLine(" [*] Waiting for messages.");
    Console.WriteLine("Press any key to exit.");
};

channel.BasicConsume(
    "task_queue",
    false,
    consumer
);

Console.WriteLine("Press any key to exit.");
Console.ReadKey();