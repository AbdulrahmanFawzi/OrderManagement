namespace OrderManagement.Application.Customers.Commands.CreateCustomer;

// Commands change state. This one carries only the input data.
// The handler performs the use case.
public record CreateCustomerCommand(string Username, string Email);
