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
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateAsync([FromBody] AddFoodItemDto dto, CancellationToken ct)
    {
        try
        {
            var result = await shoppingCartsService.AddToCartAsync(dto, ct);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            
            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Route("{userId:guid}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await shoppingCartsService.GetShoppingCartAsync(userId, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete]
    [Route("cart-items/{itemId:guid}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> DeleteCartItemAsync(Guid itemId, CancellationToken ct)
    {
        var result = await shoppingCartsService.DeleteCartItemAsync(itemId, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        
        return NoContent();
    }
}