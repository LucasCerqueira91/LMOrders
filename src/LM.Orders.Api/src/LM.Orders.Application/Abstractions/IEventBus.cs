namespace LM.Orders.Application.Abstractions;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken ct);
}
