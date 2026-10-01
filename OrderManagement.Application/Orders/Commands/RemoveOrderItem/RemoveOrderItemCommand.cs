namespace OrderManagement.Application.Orders.Commands.RemoveOrderItem;

public record RemoveOrderItemCommand(Guid OrderId, Guid OrderItemId);
