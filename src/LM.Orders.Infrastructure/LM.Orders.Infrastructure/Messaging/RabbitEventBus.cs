using LM.Orders.Application.Abstractions;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace LM.Orders.Infrastructure.Messaging;

public sealed class RabbitEventBus : IEventBus, IDisposable
{
    private readonly string _hostName;
    private readonly string _exchange = "orders.exchange";
    private readonly string _queueName = "order.created";
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitEventBus(string hostName)
    {
        _hostName = hostName;
    }

    private IModel GetChannel()
    {
        if (_channel?.IsOpen == true)
            return _channel;

        var factory = new ConnectionFactory { HostName = _hostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declara exchange e fila
        _channel.ExchangeDeclare(_exchange, ExchangeType.Direct, durable: true);
        _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(_queueName, _exchange, routingKey: "order.created");

        return _channel;
    }

    public Task PublishAsync<T>(T @event, CancellationToken ct)
    {
        try
        {
            var channel = GetChannel();
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            channel.BasicPublish(
                exchange: _exchange,
                routingKey: "order.created",
                basicProperties: props,
                body: body);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            // Fallback: log do evento em caso de falha
            Console.WriteLine($"Warning: Failed to publish event to RabbitMQ: {ex.Message}");
            Console.WriteLine($"Event: {JsonSerializer.Serialize(@event)}");
            return Task.CompletedTask;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
