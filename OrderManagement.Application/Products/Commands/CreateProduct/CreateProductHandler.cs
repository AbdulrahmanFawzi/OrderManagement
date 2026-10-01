using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Products.Dtos;
using OrderManagement.Application.Products.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Products.Commands.CreateProduct;

public class CreateProductHandler(IProductRepository products)
{
    public async Task<ProductDto> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new BusinessRuleException("Product name is required.");

        if (command.Price < 0)
            throw new BusinessRuleException("Price cannot be negative.");

        if (command.Stock < 0)
            throw new BusinessRuleException("Stock cannot be negative.");

        var product = new Product(command.Name.Trim(), command.Price, command.Stock);

        await products.AddAsync(product, cancellationToken);
        await products.SaveChangesAsync(cancellationToken);

        return new ProductDto(product.Id, product.Name, product.Price, product.Stock, product.IsActive);
    }
}
