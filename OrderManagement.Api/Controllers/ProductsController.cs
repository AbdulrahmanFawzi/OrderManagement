using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Products.Commands.ActivateProduct;
using OrderManagement.Application.Products.Commands.AdjustStock;
using OrderManagement.Application.Products.Commands.ChangeProductPrice;
using OrderManagement.Application.Products.Commands.CreateProduct;
using OrderManagement.Application.Products.Commands.DeactivateProduct;
using OrderManagement.Application.Products.Queries.GetProductById;
using OrderManagement.Application.Products.Queries.GetProducts;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(
    CreateProductHandler create,
    GetProductByIdHandler getById,
    GetProductsHandler getAll,
    ChangeProductPriceHandler changePrice,
    IncreaseStockHandler increaseStock,
    DecreaseStockHandler decreaseStock,
    ActivateProductHandler activate,
    DeactivateProductHandler deactivate) : ControllerBase
{
    public record CreateProductRequest(string Name, decimal Price, int Stock);
    public record ChangePriceRequest(decimal NewPrice);
    public record AdjustStockRequest(int Quantity);

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken ct)
    {
        var dto = await create.Handle(
            new CreateProductCommand(request.Name, request.Price, request.Stock), ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await getAll.Handle(new GetProductsQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await getById.Handle(new GetProductByIdQuery(id), ct));

    [HttpPut("{id:guid}/price")]
    public async Task<IActionResult> ChangePrice(
        Guid id, [FromBody] ChangePriceRequest request, CancellationToken ct)
    {
        await changePrice.Handle(new ChangeProductPriceCommand(id, request.NewPrice), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/stock/increase")]
    public async Task<IActionResult> IncreaseStock(
        Guid id, [FromBody] AdjustStockRequest request, CancellationToken ct)
    {
        await increaseStock.Handle(new IncreaseStockCommand(id, request.Quantity), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/stock/decrease")]
    public async Task<IActionResult> DecreaseStock(
        Guid id, [FromBody] AdjustStockRequest request, CancellationToken ct)
    {
        await decreaseStock.Handle(new DecreaseStockCommand(id, request.Quantity), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        await activate.Handle(new ActivateProductCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await deactivate.Handle(new DeactivateProductCommand(id), ct);
        return NoContent();
    }
}
