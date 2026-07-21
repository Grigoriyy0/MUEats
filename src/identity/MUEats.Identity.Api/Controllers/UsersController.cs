using Microsoft.AspNetCore.Mvc;
using MUEats.Identity.Application.Interfaces;
using MUEats.Identity.Application.Queries;

namespace MUEats.Identity.Api.Controllers;

[ApiController]
[Route("api/identity/users")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersAsync([FromQuery] GetUsersQuery query, CancellationToken ct)
    {
        return Ok(await _usersService.GetByFilterAsync(query, ct));
    }
}