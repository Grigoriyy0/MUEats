using CSharpFunctionalExtensions;
using MUEats.Application.Dto.Role;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Core.Domain.User.Entities;
using Primitives;

namespace MUEats.Application.Services;

public class RolesService : IRolesService
{
    private readonly IRolesRepository _rolesRepository;
    private readonly IUnitOfWork _uow;
    private readonly IUsersRepository _usersRepository;

    public RolesService(IUnitOfWork uow,
        IRolesRepository rolesRepository,
        IUsersRepository usersRepository)
    {
        _uow = uow;
        _rolesRepository = rolesRepository;
        _usersRepository = usersRepository;
    }

    public async Task<UnitResult<Error>> CreateAsync(string roleName, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);

        var roleExists = await _rolesRepository.AnyAsync(roleName, ct);

        if (roleExists)
        {
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.Role.RoleAlreadyExists;
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            RoleName = roleName
        };

        await _rolesRepository.AddAsync(role, ct);

        await _uow.SaveChangesAsync(ct);
        await _uow.CommitTransactionAsync(ct);

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> GrantRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        
        var role = await _rolesRepository.GetByIdAsync(roleId, ct);

        if (role is null)
        {
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.Role.RoleNotFound;
        }

        var user = await _usersRepository.GetByIdAsync(userId, ct);

        if (user is null)
        {
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.User.NotFound;
        }

        user.UserRoles.Add(new UserRole
        {
            RoleId = roleId,
            UserId = userId
        });

        await _uow.SaveChangesAsync(ct);
        await _uow.CommitTransactionAsync(ct);

        return UnitResult.Success<Error>();
    }

    public Task<List<RoleDto>> GetRolesAsync(CancellationToken ct)
    {
        return _rolesRepository.GetAllDtoAsync(ct);
    }
}