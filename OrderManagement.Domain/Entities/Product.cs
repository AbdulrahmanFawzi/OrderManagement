namespace OrderManagement.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public bool IsActive { get; private set; }

    // For EF Core only. Application code must use the public constructor.
    private Product()
    {
    }

    public Product(string name, decimal price, int stock)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Stock = stock;
        IsActive = stock > 0;
    }


    public void ChangePrice(decimal newPrice)
    {
        if (newPrice < 0)
        {
            throw new ArgumentException("Price cannot be negative.");
        }

        Price = newPrice;
    }


    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        Stock += quantity;

        if (Stock > 0)
        {
            IsActive = true;
        }
    }


    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        if (quantity > Stock)
        {
            throw new InvalidOperationException("Insufficient stock.");
        }

        Stock -= quantity;

        if (Stock == 0)
        {
            IsActive = false;
        }
    }


    public void Activate()
    {
        IsActive = true;
    }


    public void Deactivate()
    {
        IsActive = false;
    }
}