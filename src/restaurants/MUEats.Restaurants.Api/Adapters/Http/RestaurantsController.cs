using Microsoft.AspNetCore.Mvc;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Services;

namespace MUEats.Restaurants.Api.Adapters.Http;

[Route("api/restaurants")]
[ApiController]
public class RestaurantsController : ControllerBase
{
    private readonly RestaurantsService _restaurantsService;

    public RestaurantsController(RestaurantsService restaurantsService)
    {
        _restaurantsService = restaurantsService;
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