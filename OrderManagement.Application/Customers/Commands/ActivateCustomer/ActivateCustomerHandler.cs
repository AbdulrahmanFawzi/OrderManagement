using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Customers.Repositories;

namespace OrderManagement.Application.Customers.Commands.ActivateCustomer;

public class ActivateCustomerHandler(ICustomerRepository customers)
{
    public async Task Handle(
        ActivateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var customer = await customers.GetByIdAsync(command.CustomerId, cancellationToken)
            ?? throw new NotFoundException($"Customer {command.CustomerId} not found.");

        customer.Activate(); // domain behavior, no business logic here
        customers.Update(customer);
        await customers.SaveChangesAsync(cancellationToken);
    }
}
