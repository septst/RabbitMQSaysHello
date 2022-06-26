using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

Console.WriteLine("EmitLog");

channel.ExchangeDeclare(
    "logs",
    ExchangeType.Fanout
);

var message = GetMessage(args);

do
{
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        "logs",
        "",
        null,
        body
    );

    Console.WriteLine($"[x] sent {message} message.");
    Console.WriteLine("[x] Please type another message or no to quit sending messages.");
    message = Console.ReadLine();
} while (!string.IsNullOrEmpty(message) &&
         !message.ToLower().Contains("no"));

static string GetMessage(string[] args)
{
    return args.Length > 0
        ? string.Join(" ", args)
        : "info: Hello World!";
}