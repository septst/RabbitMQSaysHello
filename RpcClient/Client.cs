using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static System.Console;

namespace RpcClient;

public class Client
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly EventingBasicConsumer _consumer;
    private readonly IBasicProperties _props;
    private readonly string _replyQueueName = "reply_queue";
    private readonly string _requestQueue = "request_queue";
    private readonly BlockingCollection<string> _responseQueue = new();

    public Client()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(
            _replyQueueName,
            false,
            false,
            false,
            null
        );
        _consumer = new EventingBasicConsumer(_channel);

        var correlationId = Guid.NewGuid().ToString();
        _props = _channel.CreateBasicProperties();
        _props.CorrelationId = correlationId;
        _props.ReplyTo = _replyQueueName;

        _consumer.Received += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            WriteLine($"[x] Received response {response}");

            if (ea.BasicProperties.CorrelationId == correlationId)
                _responseQueue.Add(response);
            else
                Error.WriteLine("[x] CorrelationId did not match.");

            _channel.BasicAck(
                ea.DeliveryTag,
                false
            );
        };

        _channel.BasicConsume(
            _consumer,
            _replyQueueName
        );
    }

    public string Call(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        WriteLine($"[x] Sending request for {message}");
        _channel.BasicPublish(
            "",
            _requestQueue,
            _props,
            messageBytes
        );
        WriteLine("[x] Awaiting response...");
        return _responseQueue.Take();
    }

    public void Close()
    {
        _connection.Close();
    }
}