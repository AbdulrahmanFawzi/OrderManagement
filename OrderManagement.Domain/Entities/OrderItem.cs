namespace OrderManagement.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    // For EF Core only. Application code must use the public constructor.
    private OrderItem()
    {
    }

    public OrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        if (unitPrice < 0)
        {
            throw new ArgumentException("Unit price cannot be negative.");
        }

        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }


    public void ChangeQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        Quantity = quantity;
    }
}