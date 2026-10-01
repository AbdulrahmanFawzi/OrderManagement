using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Customers.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Repositories;

// Concrete EF Core implementation of the Application-owned interface.
public class CustomerRepository(OrderManagementDbContext db) : ICustomerRepository
{
    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
        => await db.Customers.AddAsync(customer, cancellationToken);

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => db.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
        => await db.Customers.AsNoTracking().OrderBy(c => c.Username).ToListAsync(cancellationToken);

    public Task<bool> ExistsByUsernameOrEmailAsync(
        string username,
        string email,
        CancellationToken cancellationToken = default)
        => db.Customers.AnyAsync(
            c => c.Username == username || c.Email == email,
            cancellationToken);

    public void Update(Customer customer)
        => db.Customers.Update(customer);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => db.SaveChangesAsync(cancellationToken);
}
