using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Customers.Dtos;
using OrderManagement.Application.Customers.Repositories;

namespace OrderManagement.Application.Customers.Queries.GetCustomerById;

public class GetCustomerByIdHandler(ICustomerRepository customers)
{
    public async Task<CustomerDto> Handle(
        GetCustomerByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var customer = await customers.GetByIdAsync(query.CustomerId, cancellationToken)
            ?? throw new NotFoundException($"Customer {query.CustomerId} not found.");

        return new CustomerDto(customer.Id, customer.Username, customer.Email, customer.IsActive, customer.CreatedAt);
    }
}
