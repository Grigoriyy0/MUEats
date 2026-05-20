using MUEats.Application.Dto.User;
using MUEats.Application.Responses;

namespace MUEats.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponse> AuthAsync(AuthDto dto, CancellationToken ct);
    
    Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct);
    
    Task RegisterAsync(CreateUserDto dto, CancellationToken ct);
}