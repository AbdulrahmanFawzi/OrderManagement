using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Products.Repositories;

namespace OrderManagement.Application.Products.Commands.ActivateProduct;

public class ActivateProductHandler(IProductRepository products)
{
    public async Task Handle(
        ActivateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await products.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product {command.ProductId} not found.");

        product.Activate();
        products.Update(product);
        await products.SaveChangesAsync(cancellationToken);
    }
}
