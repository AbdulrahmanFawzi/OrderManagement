using OrderManagement.Application.Customers.Repositories;
using OrderManagement.Application.Orders.Repositories;
using OrderManagement.Application.Products.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Tests.Fakes;

// In-memory fakes so Application handlers can be tested
// WITHOUT EF Core, Oracle, or Docker. Handlers depend on
// interfaces, so swapping Infrastructure for fakes is trivial -
// that is Dependency Inversion in action.
public class FakeCustomerRepository(List<Customer> store) : ICustomerRepository
{
    public Task AddAsync(Customer c, CancellationToken ct = default)
    {
        store.Add(c);
        return Task.CompletedTask;
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(store.FirstOrDefault(c => c.Id == id));

    public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Customer>>(store.ToList());

    public Task<bool> ExistsByUsernameOrEmailAsync(string u, string e, CancellationToken ct = default)
        => Task.FromResult(store.Any(c => c.Username == u || c.Email == e));

    public void Update(Customer customer) { /* in-memory: reference already updated */ }

    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
}

public class FakeProductRepository(List<Product> store) : IProductRepository
{
    public Task AddAsync(Product p, CancellationToken ct = default)
    {
        store.Add(p);
        return Task.CompletedTask;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(store.FirstOrDefault(p => p.Id == id));

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Product>>(store.ToList());

    public void Update(Product product) { }

    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
}

public class FakeOrderRepository(List<Order> store) : IOrderRepository
{
    public Task AddAsync(Order o, CancellationToken ct = default)
    {
        store.Add(o);
        return Task.CompletedTask;
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(store.FirstOrDefault(o => o.Id == id));

    public Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Order>>(store.Where(o => o.CustomerId == customerId).ToList());

    public void AddItem(OrderItem item)
    {
        // In-memory equivalent of INSERT: the item lives inside its
        // parent order's collection, so nothing extra is needed here.
        // (The real EF repository marks it Added in the change tracker.)
    }

    public void Update(Order order) { }

    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
}
