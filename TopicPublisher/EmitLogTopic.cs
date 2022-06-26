using System.Text;
using RabbitMQ.Client;
using static System.Console;

const string exchangeName = "topic_logs";
var factory = new ConnectionFactory
{
    HostName = "localhost"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchangeName,
    "topic"
);

var inputs = args;

do
{
    var routingKey = GetRoutingKey(inputs);
    var message = GetMessage(inputs);
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchangeName,
        routingKey,
        null,
        body
    );

    WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
    WriteLine("[x] Please type another message or no to quit sending messages.");
    inputs = ReadLine()?.Split(" ");
} while (inputs?.Length > 0 &&
         !inputs.Contains("no"));

string GetMessage(string[] args)
{
    return args.Length > 1
        ? string.Join(" ",
            args.Skip(1)
                .ToArray())
        : "Hello World!";
}

string GetRoutingKey(string[] args)
{
    return args.Length > 0
        ? args[0]
        : "anonymous.info";
}