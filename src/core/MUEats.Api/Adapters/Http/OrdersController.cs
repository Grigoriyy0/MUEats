using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.Order;
using MUEats.Application.Ports;
using MUEats.Application.Services;
using MUEats.Application.Services.Identity;

namespace MUEats.Adapters.Http;

[Route("api/orders")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly OrdersService _ordersService;
    private readonly IOrdersQueries _ordersQueries;

    public OrdersController(OrdersService ordersService, 
        IOrdersQueries ordersQueries)
    {
        _ordersService = ordersService;
        _ordersQueries = ordersQueries;
    }


    [HttpPost]
    [Authorize(Policy = "Customer")]
    public async Task<IActionResult> CreateAsync(CreateOrderDto dto, CancellationToken ct)
    {
        var userId = User.GetUserId();
        
        var orderId = await _ordersService.CreateAsync(userId, dto, ct);

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
        var orderStatus = await _ordersQueries.GetStatusAsync(orderId, ct);

        return Ok(orderStatus);
    }

    [HttpGet]
    [Route("{orderId:guid}")]
    [Authorize(Policy = "Customer")]
    public async Task<IActionResult> GetByIdAsync(Guid orderId, CancellationToken ct)
    {
        var orderDto = await _ordersQueries.GetDtoByIdAsync(orderId, ct);

        return Ok(orderDto);
    }

    //todo refactor: timePeriod -> FromDate, ToDate.
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
        
        return Ok(await _ordersService.GetHistoryAsync(Guid.Parse(userId), timePeriod, ct));
    }
}