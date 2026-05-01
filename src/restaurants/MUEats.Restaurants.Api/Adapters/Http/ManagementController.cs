using Microsoft.AspNetCore.Mvc;

namespace MUEats.Restaurants.Api.Adapters.Http;

[ApiController]
[Route("api/management")]
public class ManagementController : ControllerBase
{
    [HttpGet]
    [Route("menus/{menuId:guid}/items")]
    public async Task<IActionResult> GetItemsAsync([FromRoute] Guid menuId, CancellationToken ct)
    {
        return Ok();
    }
}