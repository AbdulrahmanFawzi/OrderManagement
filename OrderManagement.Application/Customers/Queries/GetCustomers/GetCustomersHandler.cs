using OrderManagement.Application.Customers.Dtos;
using OrderManagement.Application.Customers.Repositories;

namespace OrderManagement.Application.Customers.Queries.GetCustomers;

public class GetCustomersHandler(ICustomerRepository customers)
{
    public async Task<IReadOnlyList<CustomerDto>> Handle(
        GetCustomersQuery query,
        CancellationToken cancellationToken = default)
    {
        var all = await customers.GetAllAsync(cancellationToken);

        return all
            .Select(c => new CustomerDto(c.Id, c.Username, c.Email, c.IsActive, c.CreatedAt))
            .ToList();
    }
}
