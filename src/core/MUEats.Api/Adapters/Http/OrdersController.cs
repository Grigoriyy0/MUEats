using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.Order;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Application.Queries;
using MUEats.Application.Services;

namespace MUEats.Adapters.Http;

[Route("api/orders")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrdersService _ordersService;
    private readonly IOrdersQueries _ordersQueries;

    public OrdersController(IOrdersService ordersService, 
        IOrdersQueries ordersQueries)
    {
        _ordersService = ordersService;
        _ordersQueries = ordersQueries;
    }


    [HttpPost]
    [Authorize(Policy = "Customer")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var orderId = await _ordersService.CreateAsync(dto, ct);

        return Accepted(new
        {
            OrderId = orderId
        });
    }

    [HttpPut]
    [Authorize(Policy = "Customer")]
    [Route("{orderId:guid}/cancel")]
    public async Task<IActionResult> CancelAsync([FromRoute] Guid orderId, CancellationToken ct)
    {
        try
        {
            await _ordersService.CancelAsync(orderId, ct);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Route("{orderId:guid}/status")]
    [Authorize(Policy = "Customer")]
    public async Task<IActionResult> GetStatusAsync([FromRoute] Guid orderId, CancellationToken ct)
    {
        var orderStatus = await _ordersQueries.GetStatusAsync(orderId, ct);

        return Ok(orderStatus);
    }

    [HttpGet]
    [Route("{orderId:guid}")]
    [Authorize(Policy = "Customer")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid orderId, CancellationToken ct)
    {
        var orderDto = await _ordersQueries.GetDtoByIdAsync(orderId, ct);

        return Ok(orderDto);
    }
    
    [HttpGet]
    [Route("history")]
    [Authorize(Policy = "Customer")]
    public async Task<IActionResult> GetHistory([FromQuery] GetOrdersHistoryQuery query, CancellationToken ct)
    {
        return Ok(await _ordersQueries.GetHistoryAsync(query, ct));
    }
}