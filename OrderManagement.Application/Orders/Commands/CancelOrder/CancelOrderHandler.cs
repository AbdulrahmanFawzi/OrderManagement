using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Orders.Dtos;
using OrderManagement.Application.Orders.Repositories;
using OrderManagement.Application.Products.Repositories;

namespace OrderManagement.Application.Orders.Commands.CancelOrder;

public class CancelOrderHandler(
    IOrderRepository orders,
    IProductRepository products)
{
    public async Task<OrderDto> Handle(
        CancelOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order {command.OrderId} not found.");

        order.Cancel(); // throws unless Pending or Confirmed

        // Release all reserved stock.
        foreach (var item in order.OrderItems)
        {
            var product = await products.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is not null)
            {
                product.IncreaseStock(item.Quantity);
                products.Update(product);
            }
        }

        orders.Update(order);
        await orders.SaveChangesAsync(cancellationToken);

        return OrderMapping.ToDto(order);
    }
}
