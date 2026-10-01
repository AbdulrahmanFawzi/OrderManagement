using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Orders.Dtos;
using OrderManagement.Application.Orders.Repositories;
using OrderManagement.Application.Products.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.AddOrderItem;

// Cross-aggregate coordination example:
// - Order owns "pending only" + "snapshot price" rules.
// - Product owns "stock cannot go negative" rule.
// - The handler loads both and wires them together.
// Stock reservation model: adding an item immediately decreases
// product stock; removing/cancelling restores it.
public class AddOrderItemHandler(
    IOrderRepository orders,
    IProductRepository products)
{
    public async Task<OrderDto> Handle(
        AddOrderItemCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Quantity <= 0)
            throw new BusinessRuleException("Quantity must be greater than zero.");

        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order {command.OrderId} not found.");

        var product = await products.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product {command.ProductId} not found.");

        if (!product.IsActive)
            throw new BusinessRuleException("Cannot order an inactive product.");

        if (command.Quantity > product.Stock)
            throw new BusinessRuleException(
                $"Insufficient stock for '{product.Name}'. Available: {product.Stock}.");

        // Historical price snapshot: UnitPrice is copied now,
        // later Product.Price changes do not affect this item.
        var item = new OrderItem(product.Id, command.Quantity, product.Price);

        product.DecreaseStock(command.Quantity); // throws if stock would go negative
        order.AddItem(item); // throws if order is not pending

        // Domain first, then persistence: the item is new, so it must be
        // registered as Added explicitly. Relying on DetectChanges alone
        // would misclassify it as Modified (its Guid is already set),
        // producing UPDATE ... WHERE Id = @p -> 0 rows -> concurrency error.
        orders.AddItem(item);

        products.Update(product);
        orders.Update(order);
        await orders.SaveChangesAsync(cancellationToken);
        // NOTE: both repositories share the same DbContext instance,
        // so one SaveChanges persists Customer/Product/Order changes together.

        return OrderMapping.ToDto(order);
    }
}
