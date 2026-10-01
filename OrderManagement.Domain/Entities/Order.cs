using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public List<OrderItem> OrderItems { get; private set; } = new();


    private Order()
    {
    }

    public Order(Guid customerId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }


    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Items can only be added to a pending order.");
        }

        OrderItems.Add(item);
    }


    public void RemoveItem(Guid orderItemId)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Items can only be removed from a pending order.");
        }

        var item = OrderItems.FirstOrDefault(x => x.Id == orderItemId);

        if (item is null)
        {
            throw new InvalidOperationException("Order item not found.");
        }

        OrderItems.Remove(item);
    }


    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending orders can be confirmed.");
        }

        if (OrderItems.Count == 0)
        {
            throw new InvalidOperationException(
                "An order must contain at least one item.");
        }

        Status = OrderStatus.Confirmed;
    }


    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Only confirmed orders can start processing.");
        }

        Status = OrderStatus.Processing;
    }


    public void Ship()
    {
        if (Status != OrderStatus.Processing)
        {
            throw new InvalidOperationException(
                "Only processing orders can be shipped.");
        }

        Status = OrderStatus.Shipped;
    }


    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException(
                "Only shipped orders can be delivered.");
        }

        Status = OrderStatus.Delivered;
    }


    public void Cancel()
    {
        if (Status != OrderStatus.Pending &&
            Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Only pending or confirmed orders can be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }
}