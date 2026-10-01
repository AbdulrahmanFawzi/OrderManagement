namespace OrderManagement.Application.Products.Commands.ChangeProductPrice;

public record ChangeProductPriceCommand(Guid ProductId, decimal NewPrice);
