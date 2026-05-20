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
        try
        {
            await userService.CreateAsync(dto, ct);

            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> LoginAsync(AuthDto dto, CancellationToken ct)
    {
        var tokenPair = await userService.AuthAsync(dto, ct);

        Response.Cookies.Append("refreshToken", tokenPair.RefreshToken);
        
        return Ok(new
        {
            tokenPair.AccessToken
        });
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> RefreshAsync(CancellationToken ct)
    {
        var oldRefreshToken = Request.Cookies["refreshToken"];

        if (oldRefreshToken is null)
        {
            return Unauthorized();
        }

        try
        {
            var tokenPair = await userService.RefreshAsync(oldRefreshToken, ct);

            return Ok(new
            {
                tokenPair.AccessToken
            });
        }
        catch (Exception)
        {
            return Unauthorized();
        }
    }
    
}