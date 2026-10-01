using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Orders.Dtos;

public record OrderItemDto(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemDto> Items,
    decimal TotalAmount);
