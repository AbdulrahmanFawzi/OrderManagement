using OrderManagement.Application.Products.Dtos;
using OrderManagement.Application.Products.Repositories;

namespace OrderManagement.Application.Products.Queries.GetProducts;

public class GetProductsHandler(IProductRepository products)
{
    public async Task<IReadOnlyList<ProductDto>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken = default)
    {
        var all = await products.GetAllAsync(cancellationToken);

        return all
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock, p.IsActive))
            .ToList();
    }
}
