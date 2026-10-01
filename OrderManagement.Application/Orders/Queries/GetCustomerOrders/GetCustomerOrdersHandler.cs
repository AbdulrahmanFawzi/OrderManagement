using OrderManagement.Application.Orders.Dtos;
using OrderManagement.Application.Orders.Repositories;

namespace OrderManagement.Application.Orders.Queries.GetCustomerOrders;

public class GetCustomerOrdersHandler(IOrderRepository orders)
{
    public async Task<IReadOnlyList<OrderDto>> Handle(
        GetCustomerOrdersQuery query,
        CancellationToken cancellationToken = default)
    {
        var list = await orders.GetByCustomerIdAsync(query.CustomerId, cancellationToken);

        return list.Select(OrderMapping.ToDto).ToList();
    }
}
