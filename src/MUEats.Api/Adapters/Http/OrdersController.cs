using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.Order;
using MUEats.Application.Services;

namespace MUEats.Adapters.Http;

[Route("api/orders")]
[ApiController]
public class OrdersController(OrdersService ordersService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "Customer")]
    public async Task<IActionResult> CreateAsync(CreateOrderDto dto, CancellationToken ct)
    {
        var orderId = await ordersService.CreateAsync(dto, ct);

        return Accepted(new
        {
            OrderId = orderId
        });
    }

    [HttpGet]
    [Route("{orderId:guid}/status")]
    [Authorize(Policy = "Customer")]
    public async Task<IActionResult> GetStatusAsync(Guid orderId, CancellationToken ct)
    {
        var orderStatus = await ordersService.GetStatusAsync(orderId, ct);

        return Ok(orderStatus);
    }

    [HttpGet]
    [Route("{orderId:guid}")]
    [Authorize(Policy = "Customer")]
    public async Task<IActionResult> GetByIdAsync(Guid orderId, CancellationToken ct)
    {
        var orderDto = await ordersService.GetByIdAsync(orderId, ct);

        return Ok(orderDto);
    }

    [HttpGet]
    [Route("history")]
    [Authorize(Policy = "Customer")]
    public async Task<IActionResult> GetHistory([FromQuery] string timePeriod, CancellationToken ct)
    {
        var userId = User.FindFirstValue("id");

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        return Ok(await ordersService.GetHistoryAsync(Guid.Parse(userId), timePeriod, ct));
    }
}