using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Products.Repositories;

namespace OrderManagement.Application.Products.Commands.ChangeProductPrice;

public class ChangeProductPriceHandler(IProductRepository products)
{
    public async Task Handle(
        ChangeProductPriceCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await products.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product {command.ProductId} not found.");

        // Negative-price check lives inside Product.ChangePrice (domain invariant).
        // The handler just coordinates: load -> behavior -> save.
        product.ChangePrice(command.NewPrice);

        products.Update(product);
        await products.SaveChangesAsync(cancellationToken);
    }
}
