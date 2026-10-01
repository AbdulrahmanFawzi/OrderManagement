namespace OrderManagement.Domain.ValueObjects;

public class Address
{
    public string Street { get; private set; }

    public string City { get; private set; }

    public string Country { get; private set; }

    public string PostalCode { get; private set; }


    public Address(
        string street,
        string city,
        string country,
        string postalCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty.");

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.");

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty.");

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty.");

        Street = street;
        City = city;
        Country = country;
        PostalCode = postalCode;
    }
}