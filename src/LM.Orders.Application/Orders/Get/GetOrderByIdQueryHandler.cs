using LM.Orders.Application.Abstractions;
using MediatR;

namespace LM.Orders.Application.Orders.Get;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, GetOrderDto>
{
    private readonly IOrderSqlRepository _sqlRepo;
    private readonly IOrderItemsMongoRepository _mongoRepo;
    private readonly ICacheService _cache;

    public GetOrderByIdQueryHandler(
        IOrderSqlRepository sqlRepo,
        IOrderItemsMongoRepository mongoRepo,
        ICacheService cache)
    {
        _sqlRepo = sqlRepo;
        _mongoRepo = mongoRepo;
        _cache = cache;
    }

    public async Task<GetOrderDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var cacheKey = $"order:{request.Id}";
        var cached = await _cache.GetAsync<GetOrderDto>(cacheKey, ct);
        if (cached is not null)
            return cached;

        var order = await _sqlRepo.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Pedido não encontrado");

        var items = await _mongoRepo.GetByOrderIdAsync(order.Id, ct);

        var dto = new GetOrderDto(
            order.Id,
            order.CustomerId,
            order.CreatedAt,
            order.Status.ToString(),
            order.TotalAmount,
            items.Select(i => new GetOrderItemDto(i.Product, i.Quantity, i.UnitPrice)).ToList()
        );

        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(2), ct);
        return dto;
    }
}
