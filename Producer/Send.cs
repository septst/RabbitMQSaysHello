using System.Text;
using RabbitMQ.Client;

ConsoleKeyInfo cki;
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare(
    "hello",
    false,
    false,
    false,
    null
);

var message = "Hello World";
var body = Encoding.UTF8.GetBytes(message);

do
{
    channel.BasicPublish(
        "",
        "hello",
        null,
        body
    );

    Console.WriteLine($"[x] sent {message} message.");
    Console.WriteLine("[x] sending again...");
    Console.WriteLine("Press Escape or No to quit sending messages");
    cki = Console.ReadKey();
} while (cki.Key != ConsoleKey.Escape &&
         cki.Key != ConsoleKey.N);