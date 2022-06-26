using System.Text;
using RabbitMQ.Client;
using static System.Console;

ConsoleKeyInfo cki;
const string queueName = "hello";
const string message = "Hello World";
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queueName,
    false,
    false,
    false,
    null
);

channel.ConfirmSelect();
channel.BasicAcks += (_, ea) =>
{
    WriteLine(
        $"Message has been ack-ed. Sequence number: {ea.DeliveryTag}, multiple: {ea.Multiple}");
};

channel.BasicNacks += (_, ea) =>
{
    Error.WriteLine(
        $"Message has been nack-ed. Sequence number: {ea.DeliveryTag}, multiple: {ea.Multiple}");
};

var body = Encoding.UTF8.GetBytes(message);
do
{
    channel.BasicPublish(
        "",
        queueName,
        null,
        body
    );

    WriteLine($"[x] sent {message} message.");
    WriteLine("[x] sending again...");
    WriteLine("Press Escape or No to quit sending messages");
    cki = ReadKey();
} while (cki.Key != ConsoleKey.Escape &&
         cki.Key != ConsoleKey.N);