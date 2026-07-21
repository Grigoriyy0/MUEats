using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Application.Interfaces;
using MUEats.Identity.Application.Ports;
using MUEats.Identity.Application.Queries;

namespace MUEats.Identity.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public Task<List<UserDto>> GetByFilterAsync(GetUsersQuery query, CancellationToken ct)
    {
        if (query.Page < 0)
        {
            query.Page = 1;
        }

        if (query.PageSize < 0)
        {
            query.PageSize = 10;
        }

        return _usersRepository.GetUsersAsync(query, ct);
    }
}