using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.ValueObjects;

public class Money
{
    public decimal Amount { get; private set; }

    public Currency Currency { get; private set; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.");
        }

        Amount = amount;
        Currency = currency;
    }
}