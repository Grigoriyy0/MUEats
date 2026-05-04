using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.ShoppingCart;
using MUEats.Application.Services;

namespace MUEats.Adapters.Http;

[Route("api/carts")]
[ApiController]
public class ShoppingCartsController(ShoppingCartsService shoppingCartsService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateAsync([FromBody] AddFoodItemDto dto, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue("id");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            
            await shoppingCartsService.AddToCartAsync(Guid.Parse(userId), dto, ct);
            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Route("{userId:guid}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid userId, CancellationToken ct)
    {
        return Ok(await shoppingCartsService.GetShoppingCartAsync(userId, ct));
    }

    [HttpDelete]
    [Route("cart-items/{itemId:guid}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DeleteCartItemAsync(Guid itemId, CancellationToken ct)
    {
        await shoppingCartsService.DeleteCartItemAsync(itemId, ct);
        return NoContent();
    }
}