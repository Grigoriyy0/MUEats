using Microsoft.AspNetCore.Mvc;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Services;

namespace MUEats.Restaurants.Api.Adapters.Http;

[Route("api/menus")]
[ApiController]
public class MenusController : ControllerBase
{
    private readonly MenusService _menusService;

    public MenusController(MenusService menusService)
    {
        _menusService = menusService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Guid restaurantId, CancellationToken ct)
    {
        var result = await _menusService.CreateAsync(restaurantId, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Created();
    }

    [HttpPost]
    [Route("{menuId:guid}/items")]
    public async Task<IActionResult> CreateItemAsync(
        [FromRoute] Guid menuId,
        [FromBody] CreateMenuItemDto dto, 
        CancellationToken ct)
    {
        var result = await _menusService.AddMenuItemAsync(menuId, dto, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        
        return Created();
    }

    [HttpPut]
    [Route("{menuId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItemAsync(
        [FromRoute] Guid menuId,
        [FromRoute] Guid itemId,
        [FromBody] UpdateMenuItemDto dto, 
        CancellationToken ct)
    {
        var result = await _menusService.UpdateMenuItemAsync(menuId, itemId, dto, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        
        return Created();
    }

    [HttpPost]
    [Route("{menuId:guid}/categories")]
    public async Task<IActionResult> CreateCategoryAsync(
        [FromRoute] Guid menuId,
        [FromBody] CreateCategoryDto dto, 
        CancellationToken ct)
    {
        var result = await _menusService.AddCategoryAsync(menuId, dto, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        
        return Created();
    }
    
    [HttpGet]
    [Route("{restaurantId:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid restaurantId, CancellationToken ct)
    {
        return Ok(await _menusService.GetDtoByIdAsync(restaurantId, ct));
    }
}