using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Application.Interfaces;

namespace MUEats.Identity.Api.Controllers;

[ApiController]
[Route("api/identity/roles")]
[Authorize(Roles = "Admin")]
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
    
    [HttpPost]
    [Route("{userId:guid}/role/{roleId:guid}")]
    public async Task<IActionResult> GrantRoleAsync([FromBody] GrantRoleDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _rolesService.GrantRoleAsync(dto, ct);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}