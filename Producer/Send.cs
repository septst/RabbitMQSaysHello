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

var body = Encoding.UTF8.GetBytes(message);

do
{
    channel.BasicPublish(
        "",
        "hello",
        null,
        body
    );

    WriteLine($"[x] sent {message} message.");
    WriteLine("[x] sending again...");
    WriteLine("Press Escape or No to quit sending messages");
    cki = ReadKey();
} while (cki.Key != ConsoleKey.Escape &&
         cki.Key != ConsoleKey.N);