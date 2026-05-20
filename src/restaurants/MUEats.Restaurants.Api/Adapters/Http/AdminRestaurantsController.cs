using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Services;

namespace MUEats.Restaurants.Api.Adapters.Http;

[ApiController]
[Route("api/restaurants/admin")]
public class AdminRestaurantsController : ControllerBase
{
    private readonly RestaurantsService _restaurantsService;

    public AdminRestaurantsController(RestaurantsService restaurantsService)
    {
        _restaurantsService = restaurantsService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
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