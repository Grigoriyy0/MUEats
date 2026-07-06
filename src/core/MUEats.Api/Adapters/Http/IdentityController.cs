using Microsoft.AspNetCore.Mvc;
using MUEats.Application.Dto.User;
using MUEats.Application.Interfaces;

namespace MUEats.Adapters.Http;

[Route("api/auth")]
[ApiController]
public class IdentityController : ControllerBase
{
    private readonly IIdentityManager _identityManager;

    public IdentityController(IIdentityManager identityManager)
    {
        _identityManager = identityManager;
    }

    [HttpPost]
    [Route("signup")]
    public async Task<IActionResult> RegisterAsync(CreateUserDto dto, CancellationToken ct)
    {
        var result = await _identityManager.RegisterAsync(dto, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
            
        return NoContent();
    }

    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> LoginAsync(AuthDto dto, CancellationToken ct)
    {
        var tokenResult = await _identityManager.AuthAsync(dto, ct);

        if (tokenResult.IsFailure)
        {
            return BadRequest(tokenResult.Error);
        }

        var tokenPair = tokenResult.Value;
        
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
        
        var tokenPairResult = await _identityManager.RefreshAsync(oldRefreshToken, ct);

        if (tokenPairResult.IsFailure) 
        { 
            return Unauthorized(tokenPairResult.Error);
        }

        var tokenPair = tokenPairResult.Value;
            
        SetRefreshTokenCookie(tokenPair.RefreshToken);

        return Ok(new
        {
            tokenPair.AccessToken 
        });
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