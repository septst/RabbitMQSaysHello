using System.Text;
using RabbitMQ.Client;
using static System.Console;

const string exchangeName = "direct_logs";
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchangeName,
    ExchangeType.Direct
);

var inputs = args;

do
{
    var severity = GetSeverity(inputs);
    var message = GetMessage(inputs);
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchangeName,
        severity,
        null,
        body
    );

    WriteLine($"[x] sent {severity}: {message} message.");
    WriteLine("[x] Please type another message or no to quit sending messages.");
    inputs = ReadLine()?.Split(" ");
} while (inputs?.Length > 0 &&
         !inputs.Contains("no"));


static string GetSeverity(string[] args)
{
    return args.Length > 0
        ? args[0]
        : "info";
}

static string GetMessage(string[] args)
{
    return args.Length > 1
        ? string.Join(" ", args.Skip(1).ToArray())
        : "Hello World!";
}