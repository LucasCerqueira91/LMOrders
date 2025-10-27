using LM.Orders.Domain;

namespace LM.Orders.Application.Abstractions;

public interface IOrderSqlRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
}
