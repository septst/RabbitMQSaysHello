using System.Text;
using RabbitMQ.Client;
using static System.Console;

const string queueName = "task_queue";
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queueName,
    true,
    false,
    false,
    null
);

var message = "Hello World";
message = GetMessage(args);

var properties = channel.CreateBasicProperties();
properties.Persistent = true;

do
{
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish("",
        queueName,
        properties,
        body);
    WriteLine($"[x] sent {message} message.");
    WriteLine("[x] Please type another message or no to quit sending messages.");
    message = ReadLine();
} while (!string.IsNullOrEmpty(message) &&
         !message.ToLower().Contains("no"));

static string GetMessage(string[] args)
{
    return args.Length > 0 ? string.Join(" ", args) : "Hello World!";
}