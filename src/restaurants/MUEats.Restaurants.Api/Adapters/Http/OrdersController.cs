using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Restaurants.Api.Utils;
using MUEats.Restaurants.Application;
using MUEats.Restaurants.Application.Services;

namespace MUEats.Restaurants.Api.Adapters.Http;

[ApiController]
[Authorize(Roles = "RestaurantManager")]
[Route("api/management/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderSnapshotsService _service;
    private readonly CurrentUserContext _context;
    
    public OrdersController(OrderSnapshotsService service, CurrentUserContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPendingAsync(CancellationToken ct)
    {
        var restaurantId = _context.GetRestaurantId();
        
        return Ok(await _service.GetPendingAsync(restaurantId, ct));
    }

    [HttpPost]
    [Route("accept/{orderId:guid}")]
    public async Task<IActionResult> AcceptAsync([FromRoute] Guid orderId, CancellationToken ct)
    {
        var restaurantId = _context.GetRestaurantId();

        var result = await _service.AcceptAsync(orderId, restaurantId, ct);

        if (result.IsSuccess)
        {
            return Ok();
        }
        
        if (result.Error.Equals(ApplicationErrors.OrderSnapshot.WrongRestaurant))
        {
            return Forbid();
        }

        return BadRequest(result.Error.Message);
    }
    
    [HttpPost]
    [Route("reject/{orderId:guid}")]
    public async Task<IActionResult> RejectAsync([FromRoute] Guid orderId, [FromBody] string reason, CancellationToken ct)
    {
        var restaurantId = _context.GetRestaurantId();

        var result = await _service.RejectAsync(orderId, restaurantId, reason, ct);

        if (result.IsSuccess)
        {
            return Ok();
        }
        
        if (result.Error.Equals(ApplicationErrors.OrderSnapshot.WrongRestaurant))
        {
            return Forbid();
        }
        
        return BadRequest(result.Error.Message);
    }
}