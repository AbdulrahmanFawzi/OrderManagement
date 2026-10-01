using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Products.Repositories;

namespace OrderManagement.Application.Products.Commands.DeactivateProduct;

public class DeactivateProductHandler(IProductRepository products)
{
    public async Task Handle(
        DeactivateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await products.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product {command.ProductId} not found.");

        product.Deactivate();
        products.Update(product);
        await products.SaveChangesAsync(cancellationToken);
    }
}
