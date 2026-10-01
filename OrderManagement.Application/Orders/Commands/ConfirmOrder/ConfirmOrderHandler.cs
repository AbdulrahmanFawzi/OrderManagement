using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Orders.Dtos;
using OrderManagement.Application.Orders.Repositories;

namespace OrderManagement.Application.Orders.Commands.ConfirmOrder;

public class ConfirmOrderHandler(IOrderRepository orders)
{
    public async Task<OrderDto> Handle(
        ConfirmOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order {command.OrderId} not found.");

        order.Confirm(); // domain enforces: pending only + at least one item
        orders.Update(order);
        await orders.SaveChangesAsync(cancellationToken);

        return OrderMapping.ToDto(order);
    }
}
