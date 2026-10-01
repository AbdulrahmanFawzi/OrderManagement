using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Customers.Repositories;

namespace OrderManagement.Application.Customers.Commands.DeactivateCustomer;

public class DeactivateCustomerHandler(ICustomerRepository customers)
{
    public async Task Handle(
        DeactivateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var customer = await customers.GetByIdAsync(command.CustomerId, cancellationToken)
            ?? throw new NotFoundException($"Customer {command.CustomerId} not found.");

        customer.Deactivate();
        customers.Update(customer);
        await customers.SaveChangesAsync(cancellationToken);
    }
}
