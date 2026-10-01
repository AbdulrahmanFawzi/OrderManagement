using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Orders.Dtos;
using OrderManagement.Application.Orders.Repositories;

namespace OrderManagement.Application.Orders.Commands.ShipOrder;

public class ShipOrderHandler(IOrderRepository orders)
{
    public async Task<OrderDto> Handle(
        ShipOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order {command.OrderId} not found.");

        order.Ship();
        orders.Update(order);
        await orders.SaveChangesAsync(cancellationToken);

        return OrderMapping.ToDto(order);
    }
}
