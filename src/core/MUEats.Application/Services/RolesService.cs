using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Application.Services;

public class RolesService : IRolesService
{
    private readonly IUnitOfWork _uow;
    private readonly IRolesRepository _repository;

    public RolesService(IUnitOfWork uow, IRolesRepository repository)
    {
        _uow = uow;
        _repository = repository;
    }

    public async Task CreateAsync(string roleName, CancellationToken ct)
    {
        try
        {
            await _uow.BeginTransactionAsync(ct);

            var roleExists = await _repository.AnyAsync(roleName, ct);

            if (roleExists)
            {
                //todo error
                return;
            }

            var role = new Role
            {
                Id = Guid.NewGuid(),
                RoleName = roleName
            };

            await _repository.AddAsync(role, ct);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
        }
        catch (Exception)
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task GrantRoleAsync()
    {
        throw new NotImplementedException();
    }
}