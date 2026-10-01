using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Products.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Repositories;

public class ProductRepository(OrderManagementDbContext db) : IProductRepository
{
    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        => await db.Products.AddAsync(product, cancellationToken);

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => await db.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync(cancellationToken);

    public void Update(Product product)
        => db.Products.Update(product);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => db.SaveChangesAsync(cancellationToken);
}
