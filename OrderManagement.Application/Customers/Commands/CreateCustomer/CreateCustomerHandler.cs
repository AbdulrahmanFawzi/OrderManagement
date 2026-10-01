using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Customers.Dtos;
using OrderManagement.Application.Customers.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Customers.Commands.CreateCustomer;

// Primary constructor for DI (modern C# style, no boilerplate constructor).
public class CreateCustomerHandler(ICustomerRepository customers)
{
    public async Task<CustomerDto> Handle(
        CreateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        // Application validation: request shape (domain owns business invariants).
        if (string.IsNullOrWhiteSpace(command.Username))
            throw new BusinessRuleException("Username is required.");

        if (string.IsNullOrWhiteSpace(command.Email))
            throw new BusinessRuleException("Email is required.");

        // Cross-aggregate rule (uniqueness) cannot live inside Customer
        // because Customer cannot query the database. It lives here.
        if (await customers.ExistsByUsernameOrEmailAsync(command.Username, command.Email, cancellationToken))
            throw new BusinessRuleException("Username or email is already taken.");

        var customer = new Customer(command.Username.Trim(), command.Email.Trim());

        await customers.AddAsync(customer, cancellationToken);
        await customers.SaveChangesAsync(cancellationToken);

        return new CustomerDto(customer.Id, customer.Username, customer.Email, customer.IsActive, customer.CreatedAt);
    }
}
