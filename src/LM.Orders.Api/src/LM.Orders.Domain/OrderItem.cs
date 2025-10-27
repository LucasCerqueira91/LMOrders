
namespace LM.Orders.Domain;

public sealed class OrderItem
{
    
    public Guid OrderId { get; private set; }
    public string Product { get; private set; } = default!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    private OrderItem() { }
    public OrderItem(Guid orderId, string product, int quantity, decimal unitPrice)
    {
        if (orderId == Guid.Empty) throw new ArgumentException("OrderId inválido");
        if (string.IsNullOrWhiteSpace(product)) throw new ArgumentException("Produto obrigatório");
        if (quantity <= 0) throw new ArgumentException("Quantidade > 0");
        if (unitPrice < 0) throw new ArgumentException("Preço >= 0");

        OrderId = orderId;
        Product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public decimal LineTotal => Quantity * UnitPrice;
}
