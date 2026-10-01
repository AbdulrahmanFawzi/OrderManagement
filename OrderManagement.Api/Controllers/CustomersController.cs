using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Customers.Commands.ActivateCustomer;
using OrderManagement.Application.Customers.Commands.CreateCustomer;
using OrderManagement.Application.Customers.Commands.DeactivateCustomer;
using OrderManagement.Application.Customers.Queries.GetCustomerById;
using OrderManagement.Application.Customers.Queries.GetCustomers;

namespace OrderManagement.Api.Controllers;

// Thin controller: HTTP in -> handler -> HTTP out. No business logic,
// no DbContext, no domain manipulation.
[ApiController]
[Route("api/customers")]
public class CustomersController(
    CreateCustomerHandler create,
    GetCustomerByIdHandler getById,
    GetCustomersHandler getAll,
    ActivateCustomerHandler activate,
    DeactivateCustomerHandler deactivate) : ControllerBase
{
    public record CreateCustomerRequest(string Username, string Email);

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerRequest request,
        CancellationToken ct)
    {
        var dto = await create.Handle(new CreateCustomerCommand(request.Username, request.Email), ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await getAll.Handle(new GetCustomersQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await getById.Handle(new GetCustomerByIdQuery(id), ct));

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        await activate.Handle(new ActivateCustomerCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await deactivate.Handle(new DeactivateCustomerCommand(id), ct);
        return NoContent();
    }
}
