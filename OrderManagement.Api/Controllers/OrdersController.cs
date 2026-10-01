using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Orders.Commands.AddOrderItem;
using OrderManagement.Application.Orders.Commands.CancelOrder;
using OrderManagement.Application.Orders.Commands.ConfirmOrder;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Application.Orders.Commands.DeliverOrder;
using OrderManagement.Application.Orders.Commands.RemoveOrderItem;
using OrderManagement.Application.Orders.Commands.ShipOrder;
using OrderManagement.Application.Orders.Commands.StartProcessingOrder;
using OrderManagement.Application.Orders.Queries.GetCustomerOrders;
using OrderManagement.Application.Orders.Queries.GetOrderById;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(
    CreateOrderHandler create,
    AddOrderItemHandler addItem,
    RemoveOrderItemHandler removeItem,
    ConfirmOrderHandler confirm,
    CancelOrderHandler cancel,
    StartProcessingOrderHandler startProcessing,
    ShipOrderHandler ship,
    DeliverOrderHandler deliver,
    GetOrderByIdHandler getById,
    GetCustomerOrdersHandler getCustomerOrders) : ControllerBase
{
    public record CreateOrderRequest(Guid CustomerId);
    public record AddItemRequest(Guid ProductId, int Quantity);

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken ct)
    {
        var dto = await create.Handle(new CreateOrderCommand(request.CustomerId), ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await getById.Handle(new GetOrderByIdQuery(id), ct));

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetCustomerOrders(Guid customerId, CancellationToken ct)
        => Ok(await getCustomerOrders.Handle(new GetCustomerOrdersQuery(customerId), ct));

    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(
        Guid id, [FromBody] AddItemRequest request, CancellationToken ct)
        => Ok(await addItem.Handle(new AddOrderItemCommand(id, request.ProductId, request.Quantity), ct));

    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid id, Guid itemId, CancellationToken ct)
        => Ok(await removeItem.Handle(new RemoveOrderItemCommand(id, itemId), ct));

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken ct)
        => Ok(await confirm.Handle(new ConfirmOrderCommand(id), ct));

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        => Ok(await cancel.Handle(new CancelOrderCommand(id), ct));

    [HttpPost("{id:guid}/process")]
    public async Task<IActionResult> StartProcessing(Guid id, CancellationToken ct)
        => Ok(await startProcessing.Handle(new StartProcessingOrderCommand(id), ct));

    [HttpPost("{id:guid}/ship")]
    public async Task<IActionResult> Ship(Guid id, CancellationToken ct)
        => Ok(await ship.Handle(new ShipOrderCommand(id), ct));

    [HttpPost("{id:guid}/deliver")]
    public async Task<IActionResult> Deliver(Guid id, CancellationToken ct)
        => Ok(await deliver.Handle(new DeliverOrderCommand(id), ct));
}
