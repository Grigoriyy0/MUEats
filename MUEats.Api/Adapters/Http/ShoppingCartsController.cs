using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.ShoppingCart;
using MUEats.Application.Services;

namespace MUEats.Adapters.Http
{
    [Route("api/carts")]
    [ApiController]
    public class ShoppingCartsController(ShoppingCartsService shoppingCartsService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] AddFoodItemDto dto, CancellationToken ct)
        {
            try
            {
                await shoppingCartsService.AddToCartAsync(dto, ct);
                return Created();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("{userId:guid}")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid userId, CancellationToken ct)
        {
            return Ok(await shoppingCartsService.GetShoppingCartAsync(userId, ct));
        }
    }
}
