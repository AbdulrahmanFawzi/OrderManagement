namespace OrderManagement.Application.Products.Commands.AdjustStock;

public record DecreaseStockCommand(Guid ProductId, int Quantity);
