using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.Restaurant;
using MUEats.Application.Services;

namespace MUEats.Adapters.Http;

[Route("api/restaurants")]
[ApiController]
public class RestaurantsController(RestaurantsService restaurantsService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin")]
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
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{id:guid}/food-items")]
    [Authorize(Roles = "RestaurantManager")]
    public async Task<IActionResult> AddFoodItemAsync(CreateFoodItemDto dto, CancellationToken ct)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id")!.Value;

        try
        {
            await restaurantsService.AddFoodItemAsync(Guid.Parse(userId), dto, ct);

            return Created();
        }
        catch (UnauthorizedAccessException e)
        {
            return Forbid(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}