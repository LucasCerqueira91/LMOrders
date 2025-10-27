using LM.Orders.Application.Abstractions;
using LM.Orders.Domain;
using MediatR;

namespace LM.Orders.Application.Orders.Create;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderSqlRepository _sqlRepo;
    private readonly IOrderItemsMongoRepository _mongoRepo;
    private readonly IEventBus _bus;
    private readonly ICacheService _cache;

    public CreateOrderCommandHandler(
        IOrderSqlRepository sqlRepo,
        IOrderItemsMongoRepository mongoRepo,
        IEventBus bus,
        ICacheService cache)
    {
        _sqlRepo = sqlRepo;
        _mongoRepo = mongoRepo;
        _bus = bus;
        _cache = cache;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var statusParsed = Enum.TryParse<OrderStatus>(request.Status, true, out var status)
            ? status
            : throw new ArgumentException("Status inválido");

        var order = new Order(
            request.Id,
            request.CustomerId,
            request.CreatedAt,
            statusParsed,
            request.Items.Select(i => new OrderItem(request.Id, i.Product, i.Quantity, i.UnitPrice))
        );

        await _sqlRepo.AddAsync(order, ct);
        await _mongoRepo.AddManyAsync(order.Items, ct);
        await _bus.PublishAsync(new { Event = "OrderCreated", OrderId = order.Id }, ct);
        await _cache.RemoveAsync($"order:{order.Id}", ct);

        return order.Id;
    }
}
