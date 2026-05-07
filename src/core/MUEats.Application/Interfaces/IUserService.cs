using MUEats.Application.Dto.User;

namespace MUEats.Application.Interfaces;

public interface IUserService
{
    Task CreateManagerAsync(CreateManagerDto dto, CancellationToken ct);
    Task<List<ManagerDto>> GetManagersAsync(CancellationToken ct);
}