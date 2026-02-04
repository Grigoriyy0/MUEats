using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.Restaurant;
using MUEats.Application.Services;

namespace MUEats.Adapters.Http;

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
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        try
        {
            return Ok(await restaurantsService.GetByIdAsync(id, ct));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetRestaurantsQuery query, CancellationToken ct)
    {
        try
        {
            return Ok(await restaurantsService.GetAllAsync(query, ct));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost("{id:guid}/foods")]
    public async Task<IActionResult> AddFoodAsync(CreateFoodItemDto dto, CancellationToken ct)
    {
        try
        {
            await restaurantsService.AddFoodItemAsync(dto, ct);

            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}