namespace OrderManagement.Application.Orders.Commands.AddOrderItem;

public record AddOrderItemCommand(Guid OrderId, Guid ProductId, int Quantity);
