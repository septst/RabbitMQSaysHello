using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static System.Console;

const string requestQueue = "request_queue";
var factory = new ConnectionFactory
{
    HostName = "localhost"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    requestQueue,
    false,
    false,
    false,
    null);

channel.BasicQos(0,
    1,
    false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (_,
    ea) =>
{
    var response = string.Empty;

    var requestBody = ea.Body.ToArray();
    var requestProps = ea.BasicProperties;
    var replyProps = channel.CreateBasicProperties();
    replyProps.CorrelationId = requestProps.CorrelationId;

    try
    {
        var message = Encoding.UTF8.GetString(requestBody);
        WriteLine($"[x] Received request for {message} from queue {requestQueue}");

        var input = int.Parse(message);
        var answer = Factorial(input);
        response = answer.ToString();
    }
    catch (Exception e)
    {
        Error.WriteLine("[x] " + e.Message);
        response = string.Empty;
    }
    finally
    {
        var responseBytes = Encoding.UTF8.GetBytes(response);
        WriteLine($"[x] Publish response {response} to queue {requestProps.ReplyTo}");
        channel.BasicPublish(
            "",
            requestProps.ReplyTo,
            replyProps,
            responseBytes
        );

        channel.BasicAck(
            ea.DeliveryTag,
            false
        );
    }
};

channel.BasicConsume(
    requestQueue,
    false,
    consumer);

WriteLine("[x] Awaiting RPC requests");
WriteLine("[x] Press any key to exit.");
ReadLine();


static int Factorial(int n)
{
    return n == 1
        ? 1
        : n * Factorial(n - 1);
}