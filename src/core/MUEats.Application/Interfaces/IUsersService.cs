using MUEats.Application.Dto.User;
using MUEats.Application.Queries;

namespace MUEats.Application.Interfaces;

public interface IUsersService
{
    Task CreateAsync(CreateUserDto dto, CancellationToken ct);
    
    Task<List<UserDto>> GetFilteredAsync(GetUsersQuery query, CancellationToken ct);
}