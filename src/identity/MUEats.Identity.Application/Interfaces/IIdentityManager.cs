using CSharpFunctionalExtensions;
using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Application.Responses;
using Primitives;

namespace MUEats.Identity.Application.Interfaces;

public interface IIdentityManager
{
    Task<UnitResult<Error>> SignupAsync(SignupDto dto, CancellationToken ct);
    
    Task<Result<AuthResponse, Error>> SigninAsync(SigninDto dto, CancellationToken ct);
}