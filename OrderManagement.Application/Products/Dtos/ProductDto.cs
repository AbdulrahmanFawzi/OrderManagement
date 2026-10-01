namespace OrderManagement.Application.Products.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    int Stock,
    bool IsActive);
