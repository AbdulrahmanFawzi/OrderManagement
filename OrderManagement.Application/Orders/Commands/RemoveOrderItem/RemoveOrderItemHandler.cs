using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Orders.Dtos;
using OrderManagement.Application.Orders.Repositories;
using OrderManagement.Application.Products.Repositories;

namespace OrderManagement.Application.Orders.Commands.RemoveOrderItem;

public class RemoveOrderItemHandler(
    IOrderRepository orders,
    IProductRepository products)
{
    public async Task<OrderDto> Handle(
        RemoveOrderItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order {command.OrderId} not found.");

        var item = order.OrderItems.FirstOrDefault(i => i.Id == command.OrderItemId)
            ?? throw new NotFoundException($"Order item {command.OrderItemId} not found.");

        order.RemoveItem(command.OrderItemId); // throws if not pending

        // Restore the reserved stock.
        var product = await products.GetByIdAsync(item.ProductId, cancellationToken);
        if (product is not null)
        {
            product.IncreaseStock(item.Quantity);
            products.Update(product);
        }

        orders.Update(order);
        await orders.SaveChangesAsync(cancellationToken);

        return OrderMapping.ToDto(order);
    }
}
