using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Products.Repositories;

namespace OrderManagement.Application.Products.Commands.AdjustStock;

public class IncreaseStockHandler(IProductRepository products)
{
    public async Task Handle(
        IncreaseStockCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await products.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product {command.ProductId} not found.");

        product.IncreaseStock(command.Quantity);
        products.Update(product);
        await products.SaveChangesAsync(cancellationToken);
    }
}
