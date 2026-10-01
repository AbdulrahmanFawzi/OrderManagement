namespace OrderManagement.Application.Products.Commands.AdjustStock;

public record IncreaseStockCommand(Guid ProductId, int Quantity);
