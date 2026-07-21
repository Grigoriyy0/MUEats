using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Application.Queries;

namespace MUEats.Identity.Application.Interfaces;

public interface IUsersService
{
    Task<List<UserDto>> GetByFilterAsync(GetUsersQuery query, CancellationToken ct);
}