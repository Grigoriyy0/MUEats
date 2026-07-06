using CSharpFunctionalExtensions;
using MUEats.Application.Dto.User;
using MUEats.Application.Responses;
using Primitives;

namespace MUEats.Application.Interfaces;

public interface IIdentityManager
{
    Task<Result<TokenResponse, Error>> AuthAsync(AuthDto dto, CancellationToken ct);
    
    Task<Result<TokenResponse, Error>> RefreshAsync(string refreshToken, CancellationToken ct);

    Task<UnitResult<Error>> RegisterAsync(CreateUserDto dto, CancellationToken ct);
}