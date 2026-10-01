using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Dtos;

// Manual mapping keeps the project dependency-free (no AutoMapper)
// and makes the Entity -> DTO conversion explicit for learning.
public static class OrderMapping
{
    public static OrderDto ToDto(Order order)
    {
        var items = order.OrderItems
            .Select(i => new OrderItemDto(
                i.Id,
                i.ProductId,
                i.Quantity,
                i.UnitPrice,
                i.Quantity * i.UnitPrice))
            .ToList();

        return new OrderDto(
            order.Id,
            order.CustomerId,
            order.Status,
            order.CreatedAt,
            items,
            items.Sum(i => i.LineTotal));
    }
}
