using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderManagement.Infrastructure.Persistence;

// Used only by `dotnet ef migrations` at design time.
// At runtime the connection string comes from appsettings via DI.
public class OrderManagementDbContextFactory : IDesignTimeDbContextFactory<OrderManagementDbContext>
{
    public OrderManagementDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseOracle("User Id=ordermgmt;Password=ordermgmt;Data Source=localhost:1521/FREEPDB1")
            .Options;

        return new OrderManagementDbContext(options);
    }
}
