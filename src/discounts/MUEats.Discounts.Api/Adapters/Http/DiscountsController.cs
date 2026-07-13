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
        await _discountsService.CreateAsync(dto, ct);

        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> GetFilteredAsync([FromQuery] GetDiscountsQuery query, CancellationToken ct)
    {
        return Ok();
    }
}