using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.User;
using MUEats.Application.Services;

namespace MUEats.Adapters.Http;

[Route("api/auth")]
[ApiController]
public class AuthController(UserService userService) : ControllerBase
{
    [HttpPost]
    [Route("signup")]
    public async Task<IActionResult> RegisterAsync(CreateUserDto dto, CancellationToken ct)
    {
        await userService.CreateAsync(dto, ct);

        return Created();
    }

    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> LoginAsync(AuthDto dto, CancellationToken ct)
    {
        var token = await userService.AuthAsync(dto, ct);

        return Ok(new
        {
            AccessToken = token
        });
    }
    
}