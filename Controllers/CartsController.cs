using EcommerceAPI.Business.Abstract;
using EcommerceAPI.Core.DTOs.Carts;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartService _service;

    public CartsController(ICartService service) => _service = service;

    [HttpGet("{customerId:guid}")]
    public async Task<ActionResult<CartDto>> Get(Guid customerId, CancellationToken ct) =>
        await _service.GetActiveCartAsync(customerId, ct);

    [HttpPost("{customerId:guid}/items")]
    public async Task<IActionResult> AddItem(Guid customerId, [FromBody] AddCartItemRequest request, CancellationToken ct)
    {
        await _service.AddItemAsync(customerId, request, ct);
        return NoContent();
    }

    [HttpDelete("{customerId:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid customerId, Guid productId, CancellationToken ct)
    {
        await _service.RemoveItemAsync(customerId, productId, ct);
        return NoContent();
    }

    [HttpDelete("{customerId:guid}")]
    public async Task<IActionResult> Clear(Guid customerId, CancellationToken ct)
    {
        await _service.ClearAsync(customerId, ct);
        return NoContent();
    }
}
