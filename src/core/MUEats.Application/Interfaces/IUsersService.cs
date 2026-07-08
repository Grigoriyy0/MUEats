using CSharpFunctionalExtensions;
using MUEats.Application.Dto.User;
using MUEats.Application.Queries;
using Primitives;

namespace MUEats.Application.Interfaces;

public interface IUsersService
{
    Task<UnitResult<Error>> CreateAsync(CreateUserDto dto, CancellationToken ct);
    
    Task<List<UserDto>> GetFilteredAsync(GetUsersQuery query, CancellationToken ct);
}