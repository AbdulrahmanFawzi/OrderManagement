using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Customers.Repositories;

// Abstraction owned by Application, implemented by Infrastructure.
// This is the Dependency Inversion Principle: Application defines
// WHAT it needs, Infrastructure decides HOW (EF Core + Oracle).
public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsByUsernameOrEmailAsync(
        string username,
        string email,
        CancellationToken cancellationToken = default);

    void Update(Customer customer);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
