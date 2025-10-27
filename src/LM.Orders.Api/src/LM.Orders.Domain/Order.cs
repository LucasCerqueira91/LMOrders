
namespace LM.Orders.Domain;

public sealed class Order : Entity<Guid>
{
    public string CustomerId { get; private set; } = default!;
    public OrderStatus Status { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }
    public Order(Guid id, string customerId, DateTime createdAt, OrderStatus status, IEnumerable<OrderItem> items)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id inválido");
        if (string.IsNullOrWhiteSpace(customerId)) throw new ArgumentException("CustomerId obrigatório");
        var list = (items ?? throw new ArgumentException("Itens obrigatórios")).ToList();
        if (list.Count == 0) throw new ArgumentException("Pedido precisa de itens");
        if (list.Any(i => i.OrderId != id)) throw new ArgumentException("Itens com OrderId inconsistente");

        Id = id;
        CustomerId = customerId;
        CreatedAt = createdAt;
        Status = status;
        _items.AddRange(list);
    }
    public decimal TotalAmount => _items.Sum(i => i.LineTotal);
    public void MarkPaid() => Status = OrderStatus.Paid;
    public void Cancel() => Status = OrderStatus.Cancelled;
}
