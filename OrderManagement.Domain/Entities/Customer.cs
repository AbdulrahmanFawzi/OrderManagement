namespace OrderManagement.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }

    public string Username { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    // For EF Core only. Application code must use the public constructor.
    private Customer()
    {
    }

    public Customer(string username, string email)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
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