using Microsoft.AspNetCore.Mvc;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Services;

namespace MUEats.Restaurants.Api.Adapters.Http;

[Route("api/restaurants")]
[ApiController]
public class RestaurantsController : ControllerBase
{
    private readonly RestaurantsService _restaurantsService;

    private readonly MenusService _menusService;
    
    public RestaurantsController(RestaurantsService restaurantsService, MenusService menusService)
    {
        _restaurantsService = restaurantsService;
        _menusService = menusService;
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return Ok(await _restaurantsService.GetDtoByIdAsync(id, ct));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
    {
        return Ok(await _restaurantsService.GetAllDtosAsync(ct));
    }
    
    [HttpGet]
    [Route("{restaurantId:guid}/menu")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid restaurantId, CancellationToken ct)
    {
        return Ok(await _menusService.GetDtoByIdAsync(restaurantId, ct));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateRestaurantDto dto, CancellationToken ct)
    {
        var result = await _restaurantsService.CreateAsync(dto, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Created();
    }
}