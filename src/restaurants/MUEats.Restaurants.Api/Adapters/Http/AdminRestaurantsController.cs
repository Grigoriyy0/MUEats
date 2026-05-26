using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Restaurants.Api.Utils;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Services;

namespace MUEats.Restaurants.Api.Adapters.Http;

[ApiController]
[Route("api/restaurants/admin")]
public class AdminRestaurantsController : ControllerBase
{
    private readonly RestaurantsService _restaurantsService;
    private readonly CurrentUserContext _context;
    private readonly MenusQueries _menusQueries;
    
    public AdminRestaurantsController(RestaurantsService restaurantsService, 
        CurrentUserContext context, 
        MenusQueries menusQueries)
    {
        _restaurantsService = restaurantsService;
        _context = context;
        _menusQueries = menusQueries;
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

    [HttpGet]
    [Authorize(Roles = "RestaurantManager")]
    [Route("menus/{id:guid}")]
    public async Task<IActionResult> GetMenuAsync(Guid id, CancellationToken ct)
    {
        var restaurantId = _context.GetRestaurantId();

        if (restaurantId != id)
        {
            throw new UnauthorizedAccessException();
        }
        
        return Ok(await _menusQueries.GetAdminViewDtoByIdAsync(id, ct));
    }
}