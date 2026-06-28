using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.User;
using MUEats.Application.Interfaces;

namespace MUEats.Adapters.Http;

[Route("api/users")]
[Authorize(Roles = "Admin")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }
    
    [HttpPost]
    [Route("managers")]
    public async Task<IActionResult> CreateManagerAsync([FromBody] CreateManagerDto dto, CancellationToken ct)
    {
        try
        {
            //await  _usersService.CreateAsync(dto, ct);
            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Route("managers")]
    public async Task<IActionResult> GetManagersAsync(CancellationToken ct)
    {
        return Ok(await _usersService.GetManagersAsync(ct));
    }
}