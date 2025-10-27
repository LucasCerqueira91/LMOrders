using LM.Orders.Domain;

namespace LM.Orders.Application.Abstractions;
public interface IOrderItemsMongoRepository
{
    Task AddManyAsync(IEnumerable<OrderItem> items, CancellationToken ct);
    Task<IReadOnlyCollection<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken ct);
}
