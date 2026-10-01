using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Orders.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Repositories;

public class OrderRepository(OrderManagementDbContext db) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        => await db.Orders.AddAsync(order, cancellationToken);

    // The whole aggregate (Order + OrderItems) is loaded together.
    // External code must go through Order, never query OrderItems directly.
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
        => await db.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

    public void AddItem(OrderItem item)
        => db.OrderItems.Add(item);

    public void Update(Order order)
    {
        // Marks ONLY the Order root as modified.
        // db.Orders.Update(order) would walk the whole aggregate graph
        // and touch children's tracking states, so it is avoided here:
        // new items are registered explicitly via AddItem() above,
        // removals are detected as Deleted on SaveChanges.
        db.Entry(order).State = EntityState.Modified;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => db.SaveChangesAsync(cancellationToken);
}
