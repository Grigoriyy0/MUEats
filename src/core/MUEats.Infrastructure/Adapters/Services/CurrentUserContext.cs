using Microsoft.AspNetCore.Http;
using MUEats.Application.Ports;

namespace MUEats.Infrastructure.Adapters.Services;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid GetUserId()
    {
        var idClaim = _accessor.HttpContext?.User.FindFirst("id")?.Value;

        if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
        {
            throw new UnauthorizedAccessException();
        }

        return userId;
    }

    public bool IsAuthenticated()
    {
        return _accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}