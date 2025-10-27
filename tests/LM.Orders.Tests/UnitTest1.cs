using LM.Orders.Domain;
using Xunit;

namespace LM.Orders.Tests;

public class OrderTests
{
    [Fact]
    public void CreateOrder_WithValidData_ShouldSucceed()
    {
        var id = Guid.NewGuid();
        var customerId = "customer123";
        var createdAt = DateTime.UtcNow;
        var status = OrderStatus.Created;
        var items = new List<OrderItem>
        {
            new(id, "Product 1", 2, 10.50m),
            new(id, "Product 2", 1, 25.00m)
        };

        var order = new Order(id, customerId, createdAt, status, items);

        Assert.Equal(id, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(createdAt, order.CreatedAt);
        Assert.Equal(status, order.Status);
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(46.00m, order.TotalAmount);
    }

    [Fact]
    public void CreateOrder_WithInvalidData_ShouldThrowException()
    {
        var id = Guid.Empty;
        var customerId = "";
        var createdAt = DateTime.UtcNow;
        var status = OrderStatus.Created;
        var items = new List<OrderItem>();

        Assert.Throws<ArgumentException>(() => 
            new Order(id, customerId, createdAt, status, items));
    }

    [Fact]
    public void OrderItem_CalculateLineTotal_ShouldReturnCorrectValue()
    {
        var orderId = Guid.NewGuid();
        var product = "Test Product";
        var quantity = 3;
        var unitPrice = 15.50m;

        var item = new OrderItem(orderId, product, quantity, unitPrice);

        Assert.Equal(46.50m, item.LineTotal);
    }

    [Fact]
    public void Order_MarkPaid_ShouldChangeStatus()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(
            orderId,
            "customer123",
            DateTime.UtcNow,
            OrderStatus.Created,
            new List<OrderItem> { new(orderId, "Product", 1, 10m) }
        );

        order.MarkPaid();

        Assert.Equal(OrderStatus.Paid, order.Status);
    }
}