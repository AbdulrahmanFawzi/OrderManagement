namespace OrderManagement.Application.Customers.Queries.GetCustomerById;

// Queries only read state and never modify it (CQRS).
public record GetCustomerByIdQuery(Guid CustomerId);
