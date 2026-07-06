using MUEats.Application.Dto.User;
using MUEats.Application.Queries;
using MUEats.Core.Domain.User;

namespace MUEats.Application.Ports;

public interface IUsersRepository
{
    Task AddAsync(User user, CancellationToken ct);
    
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    
    Task<List<UserDto>> GetUsersAsync(GetUsersQuery query, CancellationToken ct);

    Task<bool> AnyAsync(string email, CancellationToken ct);
}