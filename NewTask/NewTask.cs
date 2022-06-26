using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    "task_queue",
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
        "task_queue",
        properties,
        body);
    Console.WriteLine($"[x] sent {message} message.");
    Console.WriteLine("[x] Please type another message or no to quit sending messages.");
    message = Console.ReadLine();
} while (message != null && !message.ToLower().Contains("no"));

static string GetMessage(string[] args)
{
    return args.Length > 0 ? string.Join(" ", args) : "Hello World!";
}