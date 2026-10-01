using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Products.Dtos;
using OrderManagement.Application.Products.Repositories;

namespace OrderManagement.Application.Products.Queries.GetProductById;

public class GetProductByIdHandler(IProductRepository products)
{
    public async Task<ProductDto> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var product = await products.GetByIdAsync(query.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product {query.ProductId} not found.");

        return new ProductDto(product.Id, product.Name, product.Price, product.Stock, product.IsActive);
    }
}
