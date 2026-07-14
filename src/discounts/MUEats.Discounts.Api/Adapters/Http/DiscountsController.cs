using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Discounts.Api.Utils;
using MUEats.Discounts.Application.Dtos;
using MUEats.Discounts.Application.Queries;
using MUEats.Discounts.Application.Services;

namespace MUEats.Discounts.Api.Adapters.Http;

[ApiController]
[Route("api/discounts")]
[Authorize(Roles="RestaurantOwner,Admin")]
[AuthorizeRestaurant]
public class DiscountsController : ControllerBase
{
    private readonly DiscountsService _discountsService;
    private readonly CurrentUserContext _context;

    public DiscountsController(DiscountsService discountsService, CurrentUserContext context)
    {
        _discountsService = discountsService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateDiscountDto dto, CancellationToken ct)
    {
        var createResult = await _discountsService.CreateAsync(dto, ct);

        if (createResult.IsFailure)
        {
            return BadRequest(createResult.Error);
        }
        
        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> GetByFilterAsync([FromQuery] GetDiscountsQuery query, CancellationToken ct)
    {
        return Ok(await _discountsService.GetByFilterAsync(query, ct));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateDiscountDto dto, CancellationToken ct)
    {
        var result = await _discountsService.UpdateAsync(dto, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
    
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        await _discountsService.DeleteAsync(id, ct);

        return NoContent();
    }
}