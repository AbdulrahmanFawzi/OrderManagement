using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Products.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    void Update(Product product);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
