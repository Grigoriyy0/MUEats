using MUEats.Application.Dto.Role;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Application.Services;

public class RolesService : IRolesService
{
    private readonly IUnitOfWork _uow;
    private readonly IRolesRepository _rolesRepository;
    private readonly IUsersRepository _usersRepository;

    public RolesService(IUnitOfWork uow, 
        IRolesRepository rolesRepository, 
        IUsersRepository usersRepository)
    {
        _uow = uow;
        _rolesRepository = rolesRepository;
        _usersRepository = usersRepository;
    }

    public async Task CreateAsync(string roleName, CancellationToken ct)
    {
        try
        {
            await _uow.BeginTransactionAsync(ct);

            var roleExists = await _rolesRepository.AnyAsync(roleName, ct);

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

            await _rolesRepository.AddAsync(role, ct);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
        }
        catch (Exception)
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task GrantRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        try
        {
            var role = await _rolesRepository.GetByIdAsync(roleId, ct);

            if (role is null)
            {
                //todo error
                return;
            }

            var user = await _usersRepository.GetByIdAsync(userId, ct);

            if (user is null)
            {
                //todo error
                return;
            }
            
            user.UserRoles.Add(new UserRole
            {
                RoleId = roleId,
                UserId = userId
            });

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
        }
        catch (Exception)
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public Task<List<RoleDto>> GetRolesAsync(CancellationToken ct)
    {
        return _rolesRepository.GetAllDtoAsync(ct);
    }
}