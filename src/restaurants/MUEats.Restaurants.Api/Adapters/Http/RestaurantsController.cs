using Microsoft.AspNetCore.Mvc;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Services;

namespace MUEats.Restaurants.Api.Adapters.Http;

[Route("api/restaurants")]
[ApiController]
public class RestaurantsController : ControllerBase
{
    private readonly RestaurantsService _restaurantsService;
    private readonly MenusQueries _menusQueries;
    
    public RestaurantsController(RestaurantsService restaurantsService, MenusQueries menusQueries)
    {
        _restaurantsService = restaurantsService;
        _menusQueries = menusQueries;
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
        return Ok(await _menusQueries.GetDtoByIdAsync(restaurantId, ct));
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