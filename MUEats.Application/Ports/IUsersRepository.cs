using MUEats.Core.Domain.User;

namespace MUEats.Application.Ports;

public interface IUsersRepository
{
    Task AddAsync(User user, CancellationToken ct);
    
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task DeleteAsync(User user, CancellationToken ct);
    
    Task UpdateAsync(User user, CancellationToken ct);
}