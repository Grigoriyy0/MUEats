using Microsoft.AspNetCore.Mvc;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Services;

namespace MUEats.Restaurants.Api.Adapters.Http;

[Route("api/menus")]
[ApiController]
public class MenusController : ControllerBase
{
    private readonly MenusService _menusService;
    private readonly MenusQueries _menusQueries;
    
    public MenusController(MenusService menusService, MenusQueries menusQueries)
    {
        _menusService = menusService;
        _menusQueries = menusQueries;
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

    [HttpPost]
    [Route("{menuId:guid}/items/{itemId:guid}/options-group")]
    public async Task<IActionResult> AddOptionsGroupAsync([FromRoute] Guid menuId,
        [FromRoute] Guid itemId,
        [FromBody] CreateOptionsGroupDto dto,
        CancellationToken ct)
    {
        var result = await _menusService.AddOptionsGroupAsync(menuId, itemId, dto, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Created();
    }

    [HttpPost]
    [Route("{menuId:guid}/option-groups/{groupId:guid}/options")]
    public async Task<IActionResult> AddItemOptionAsync([FromRoute] Guid menuId,
        [FromRoute] Guid groupId,
        [FromBody] AddItemOptionDto dto,
        CancellationToken ct)
    {
        var result = await _menusService.AddItemOptionAsync(menuId, groupId, dto, ct);

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

        return Ok();
    }
    
    [HttpGet]
    [Route("{menuId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> GetItemAsync([FromRoute] Guid menuId,
        [FromRoute] Guid itemId,
        CancellationToken ct)
    {
        return Ok(await _menusQueries.GetItemDtoAsync(menuId, itemId, ct)); 
    }

    [HttpDelete]
    [Route("{menuId:guid}/option-groups/{groupId:guid}")]
    public async Task<IActionResult> DeleteOptionsGroupAsync([FromRoute] Guid menuId,
        [FromRoute] Guid groupId,
        CancellationToken ct)
    {
        var result = await _menusService.DeleteOptionsGroupAsync(menuId, groupId, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpDelete]
    [Route("{menuId:guid}/item-options/{optionId:guid}")]
    public async Task<IActionResult> DeleteItemOptionAsync([FromRoute] Guid menuId,
        [FromRoute] Guid optionId,
        CancellationToken ct)
    {
        var result = await _menusService.DeleteItemOptionAsync(menuId, optionId, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}