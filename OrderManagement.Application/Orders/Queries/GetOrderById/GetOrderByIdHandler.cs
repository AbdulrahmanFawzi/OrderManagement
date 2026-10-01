using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Orders.Dtos;
using OrderManagement.Application.Orders.Repositories;

namespace OrderManagement.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdHandler(IOrderRepository orders)
{
    public async Task<OrderDto> Handle(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var order = await orders.GetByIdAsync(query.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order {query.OrderId} not found.");

        return OrderMapping.ToDto(order);
    }
}
