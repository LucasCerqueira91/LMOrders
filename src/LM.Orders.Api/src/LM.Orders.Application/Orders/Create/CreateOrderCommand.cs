using LM.Orders.Domain;
using MediatR;

namespace LM.Orders.Application.Orders.Create;

public sealed record CreateOrderCommand(
    Guid Id,
    string CustomerId,
    DateTime CreatedAt,
    string Status,
    IReadOnlyCollection<CreateOrderItemDto> Items
) : IRequest<Guid>;

public sealed record CreateOrderItemDto(string Product, int Quantity, decimal UnitPrice);
