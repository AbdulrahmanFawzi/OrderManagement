namespace OrderManagement.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name, decimal Price, int Stock);
