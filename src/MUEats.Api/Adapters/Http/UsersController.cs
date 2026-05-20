using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.User;
using MUEats.Application.Services;

namespace MUEats.Adapters.Http;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("{userId:guid}/{roleName}")]
    public async Task<IActionResult> GrantRoleAsync([FromRoute] Guid userId, [FromRoute] string roleName, CancellationToken ct)
    {
        try
        {
            await _userService.GrantRoleAsync(roleName, userId, ct);
            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateManagerAsync([FromBody] CreateManagerDto dto, CancellationToken ct)
    {
        try
        {
            await  _userService.CreateManagerAsync(dto, ct);
            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}