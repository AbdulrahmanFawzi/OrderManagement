using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Customers.Repositories;
using OrderManagement.Application.Orders.Repositories;
using OrderManagement.Application.Products.Repositories;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Repositories;

namespace OrderManagement.Infrastructure;

// Wires Application interfaces to Infrastructure implementations.
// Api calls this; neither Application nor Domain knows about it.
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<OrderManagementDbContext>(options =>
            options.UseOracle(connectionString));

        // Same DbContext instance is shared per request (Scoped),
        // so a handler updating both Product and Order can call
        // one SaveChanges and persist everything atomically.
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
