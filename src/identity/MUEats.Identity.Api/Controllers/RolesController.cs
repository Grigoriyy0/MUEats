using Microsoft.AspNetCore.Mvc;
using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Application.Interfaces;

namespace MUEats.Identity.Api.Controllers;

[ApiController]
[Route("api/identity/roles")]
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
    public async Task<IActionResult> CreateAsync([FromBody] CreateRoleDto dto, CancellationToken ct)
    {
        var roleResult = await _rolesService.CreateAsync(dto, ct);

        if (roleResult.IsFailure)
        {
            return BadRequest(roleResult.Error);
        }
        
        return Created();
    }
}