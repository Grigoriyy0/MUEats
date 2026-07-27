using Microsoft.AspNetCore.Mvc;
using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Application.Interfaces;

namespace MUEats.Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IIdentityManager _identityManager;

    public AuthController(IIdentityManager identityManager)
    {
        _identityManager = identityManager;
    }

    [HttpPost]
    [Route("signup")]
    public async Task<IActionResult> SignupAsync([FromBody] SignupDto dto, CancellationToken ct)
    {
        var result = await _identityManager.SignupAsync(dto, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Created();
    }

    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> SigninAsync([FromBody] SigninDto dto, CancellationToken ct)
    {
        var result = await _identityManager.SigninAsync(dto, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}