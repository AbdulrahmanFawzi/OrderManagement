using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Customers.Repositories;
using OrderManagement.Application.Orders.Dtos;
using OrderManagement.Application.Orders.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

// Coordinates two aggregates (Customer + Order) without letting
// Order query the database: the handler loads the Customer,
// enforces "must be active", then creates the Order.
public class CreateOrderHandler(
    IOrderRepository orders,
    ICustomerRepository customers)
{
    public async Task<OrderDto> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var customer = await customers.GetByIdAsync(command.CustomerId, cancellationToken)
            ?? throw new NotFoundException($"Customer {command.CustomerId} not found.");

        if (!customer.IsActive)
            throw new BusinessRuleException("Inactive customer cannot create orders.");

        var order = new Order(command.CustomerId);

        await orders.AddAsync(order, cancellationToken);
        await orders.SaveChangesAsync(cancellationToken);

        return OrderMapping.ToDto(order);
    }
}
