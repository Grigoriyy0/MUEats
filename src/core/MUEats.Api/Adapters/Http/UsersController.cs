using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Interfaces;
using MUEats.Application.Queries;

namespace MUEats.Adapters.Http;

[Route("api/users")]
[Authorize(Roles = "Admin")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    private readonly IRolesService _rolesService;

    public UsersController(IUsersService usersService, IRolesService rolesService)
    {
        _usersService = usersService;
        _rolesService = rolesService;
    }
    
    [HttpPost]
    [Route("{userId:guid}/role/{roleId:guid}")]
    public async Task<IActionResult> GrantRoleAsync([FromRoute] Guid userId, Guid roleId, CancellationToken ct)
    {
        try
        {
            await _rolesService.GrantRoleAsync(userId, roleId, ct);

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersAsync([FromQuery] GetUsersQuery query, CancellationToken ct)
    {
        return Ok(await _usersService.GetManagersAsync(ct));
    }
}