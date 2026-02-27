using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.Order;
using MUEats.Application.Services;

namespace MUEats.Adapters.Http;

[Route("api/orders")]
[ApiController]
public class OrdersController(OrdersService ordersService) : ControllerBase
{
    [HttpPost]
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
    public async Task<IActionResult> GetStatusAsync(Guid orderId, CancellationToken ct)
    {
        var orderStatus = await ordersService.GetStatusAsync(orderId, ct);

        return Ok(orderStatus);
    }

    [HttpGet]
    [Route("{orderId:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid orderId, CancellationToken ct)
    {
        var orderDto = await ordersService.GetByIdAsync(orderId, ct);

        return Ok(orderDto);
    }
}