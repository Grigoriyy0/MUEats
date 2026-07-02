using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Interfaces;

namespace MUEats.Adapters.Http;

[ApiController]
[Route("api/roles")]
[Authorize(Roles="Admin")]
public class RolesController : ControllerBase
{
    private readonly IRolesService _rolesService;

    public RolesController(IRolesService rolesService)
    {
        _rolesService = rolesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRolesAsync(CancellationToken ct)
    {
        return Ok(await _rolesService.GetRolesAsync(ct));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(string roleName, CancellationToken ct)
    {
        try
        {
            await _rolesService.CreateAsync(roleName, ct);

            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}