using MongoDB.Driver;
using LM.Orders.Domain;
using LM.Orders.Application.Abstractions;

namespace LM.Orders.Infrastructure.Mongo;

public sealed class OrderItemsMongoRepository : IOrderItemsMongoRepository
{
    private readonly IMongoCollection<OrderItem> _col;

    public OrderItemsMongoRepository(IMongoDatabase db)
        => _col = db.GetCollection<OrderItem>("order_items");

    public Task AddManyAsync(IEnumerable<OrderItem> items, CancellationToken ct) =>
        _col.InsertManyAsync(items.ToList(), cancellationToken: ct);

    public async Task<IReadOnlyCollection<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken ct) =>
        await _col.Find(x => x.OrderId == orderId).ToListAsync(ct);
}
