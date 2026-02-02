using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.Restaurant;
using MUEats.Application.Services;

namespace MUEats.Adapters.Http
{
    [Route("api/restaurants")]
    [ApiController]
    public class RestaurantsController(RestaurantsService restaurantsService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateRestaurantDto dto, CancellationToken ct)
        {
            try
            {
                await restaurantsService.CreateAsync(dto, ct);
                return Created();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
