using System.Text;
using RabbitMQ.Client;
using static System.Console;

const string exchangeName = "logs";
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchangeName,
    ExchangeType.Fanout
);

var message = GetMessage(args);

do
{
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchangeName,
        "",
        null,
        body
    );

    WriteLine($"[x] sent {message} message.");
    WriteLine("[x] Please type another message or no to quit sending messages.");
    message = ReadLine();
} while (!string.IsNullOrEmpty(message) &&
         !message.ToLower().Contains("no"));

static string GetMessage(string[] args)
{
    return args.Length > 0
        ? string.Join(" ", args)
        : "info: Hello World!";
}