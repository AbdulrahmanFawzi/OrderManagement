using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    // Must include OrderItems (aggregate boundary: Order + its items load together).
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    void AddItem(OrderManagement.Domain.Entities.OrderItem item);

    void Update(Order order);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
