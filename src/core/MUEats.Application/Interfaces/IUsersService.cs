using MUEats.Application.Dto.User;

namespace MUEats.Application.Interfaces;

public interface IUsersService
{
    Task CreateAsync(CreateUserDto dto, CancellationToken ct);
    
    Task<List<ManagerDto>> GetManagersAsync(CancellationToken ct);
}