using LM.Orders.Domain;
using MediatR;

namespace LM.Orders.Application.Orders.Get;

public sealed record GetOrderByIdQuery(Guid Id) : IRequest<GetOrderDto>;

public sealed record GetOrderDto(
    Guid Id,
    string CustomerId,
    DateTime CreatedAt,
    string Status,
    decimal TotalAmount,
    IReadOnlyCollection<GetOrderItemDto> Items
);

public sealed record GetOrderItemDto(string Product, int Quantity, decimal UnitPrice);
