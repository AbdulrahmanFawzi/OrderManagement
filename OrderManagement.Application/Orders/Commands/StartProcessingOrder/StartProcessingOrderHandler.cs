using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Orders.Dtos;
using OrderManagement.Application.Orders.Repositories;

namespace OrderManagement.Application.Orders.Commands.StartProcessingOrder;

public class StartProcessingOrderHandler(IOrderRepository orders)
{
    public async Task<OrderDto> Handle(
        StartProcessingOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order {command.OrderId} not found.");

        order.StartProcessing();
        orders.Update(order);
        await orders.SaveChangesAsync(cancellationToken);

        return OrderMapping.ToDto(order);
    }
}
