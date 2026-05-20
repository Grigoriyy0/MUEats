using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.User;
using MUEats.Application.Interfaces;

namespace MUEats.Adapters.Http;

[Route("api/auth")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost]
    [Route("signup")]
    public async Task<IActionResult> RegisterAsync(CreateUserDto dto, CancellationToken ct)
    {
        try
        {
            await authService.RegisterAsync(dto, ct);

            return NoContent();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> LoginAsync(AuthDto dto, CancellationToken ct)
    {
        var tokenPair = await authService.AuthAsync(dto, ct);

        SetRefreshTokenCookie(tokenPair.RefreshToken);
        
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
            var tokenPair = await authService.RefreshAsync(oldRefreshToken, ct);

            SetRefreshTokenCookie(tokenPair.RefreshToken);
        
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
    
    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}