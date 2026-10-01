using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Tests.Domain;

// These tests document the Order aggregate's business rules.
// No database, no mocks: pure domain behavior.
public class OrderTests
{
    private static Order NewPendingOrder() => new Order(Guid.NewGuid());

    private static Order OrderWithItem(int quantity = 1, decimal price = 10m)
    {
        var order = NewPendingOrder();
        order.AddItem(new OrderItem(Guid.NewGuid(), quantity, price));
        return order;
    }

    [Fact]
    public void New_order_starts_as_pending()
    {
        var order = NewPendingOrder();
        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void Cannot_confirm_order_without_items()
    {
        var order = NewPendingOrder();
        Assert.Throws<InvalidOperationException>(() => order.Confirm());
    }

    [Fact]
    public void Can_confirm_order_with_items()
    {
        var order = OrderWithItem();
        order.Confirm();
        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void Cannot_modify_items_after_confirmation()
    {
        var order = OrderWithItem();
        order.Confirm();

        Assert.Throws<InvalidOperationException>(
            () => order.AddItem(new OrderItem(Guid.NewGuid(), 1, 5m)));
        Assert.Throws<InvalidOperationException>(
            () => order.RemoveItem(order.OrderItems[0].Id));
    }

    [Fact]
    public void Full_lifecycle_transitions_work()
    {
        var order = OrderWithItem();
        order.Confirm();
        order.StartProcessing();
        order.Ship();
        order.Deliver();
        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    [Fact]
    public void Cannot_skip_lifecycle_steps()
    {
        var order = OrderWithItem();

        // Pending -> Processing is illegal, must go through Confirmed.
        Assert.Throws<InvalidOperationException>(() => order.StartProcessing());
        // Pending -> Shipped is illegal.
        Assert.Throws<InvalidOperationException>(() => order.Ship());
    }

    [Theory]
    // Cancellation allowed from Pending and Confirmed...
    [InlineData(OrderStatus.Pending, true)]
    [InlineData(OrderStatus.Confirmed, true)]
    // ...but not after shipping starts.
    [InlineData(OrderStatus.Processing, false)]
    [InlineData(OrderStatus.Shipped, false)]
    [InlineData(OrderStatus.Delivered, false)]
    public void Cancellation_rules(OrderStatus from, bool allowed)
    {
        var order = OrderWithItem();
        Advance(order, from);

        if (allowed)
        {
            order.Cancel();
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }
        else
        {
            Assert.Throws<InvalidOperationException>(() => order.Cancel());
        }
    }

    private static void Advance(Order order, OrderStatus target)
    {
        if (target is OrderStatus.Pending or OrderStatus.Cancelled)
            return;
        order.Confirm();
        if (target == OrderStatus.Confirmed)
            return;
        order.StartProcessing();
        if (target == OrderStatus.Processing)
            return;
        order.Ship();
        if (target == OrderStatus.Shipped)
            return;
        order.Deliver();
    }
}
