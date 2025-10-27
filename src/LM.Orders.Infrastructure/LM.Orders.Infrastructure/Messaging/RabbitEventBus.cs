using LM.Orders.Application.Abstractions;
using System.Text;
using System.Text.Json;

namespace LM.Orders.Infrastructure.Messaging;

public sealed class RabbitEventBus : IEventBus, IDisposable
{
    private readonly string _hostName;
    private readonly string _exchange = "orders.exchange";

    public RabbitEventBus(string hostName)
    {
        _hostName = hostName;
    }

    public Task PublishAsync<T>(T @event, CancellationToken ct)
    {
        // Implementação simplificada para demonstração
        // Em produção, seria necessário implementar a conexão real com RabbitMQ
        Console.WriteLine($"Event published to {_exchange}: {JsonSerializer.Serialize(@event)}");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Cleanup resources if needed
    }
}
