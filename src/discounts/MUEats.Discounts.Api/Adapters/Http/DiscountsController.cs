using Microsoft.AspNetCore.Mvc;
using MUEats.Discounts.Application.Dtos;
using MUEats.Discounts.Application.Queries;
using MUEats.Discounts.Application.Services;

namespace MUEats.Discounts.Api.Adapters.Http;

[ApiController]
[Route("api/discounts")]
public class DiscountsController : ControllerBase
{
    private readonly DiscountsService _discountsService;

    public DiscountsController(DiscountsService discountsService)
    {
        _discountsService = discountsService;
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