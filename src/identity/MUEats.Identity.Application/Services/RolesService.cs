using CSharpFunctionalExtensions;
using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Application.Interfaces;
using MUEats.Identity.Application.Ports;
using MUEats.Identity.Core.Domain.Role;
using Primitives;

namespace MUEats.Identity.Application.Services;

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

    public async Task<UnitResult<Error>> CreateAsync(CreateRoleDto dto, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);

        var roleExists = await _rolesRepository.AnyAsync(dto.RoleName, ct);

        if (roleExists)
        {
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.Role.RoleAlreadyExists;
        }

        var roleResult = Role.Create(dto.RoleName);

        if (roleResult.IsFailure)
        {
            return roleResult.Error;
        }

        var role = roleResult.Value;

        if (dto.RequiredAttributes is not null)
        {
            foreach (var attribute in dto.RequiredAttributes)
            {
                var addResult = role.AddOrUpdateRequirement(attribute);

                if (addResult.IsFailure)
                {
                    await _uow.RollbackTransactionAsync(ct);

                    return addResult.Error;
                }
            }
        }

        await _rolesRepository.AddAsync(role, ct);

        await _uow.SaveChangesAsync(ct);
        await _uow.CommitTransactionAsync(ct);

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> GrantRoleAsync(GrantRoleDto dto, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        
        var role = await _rolesRepository.GetByIdAsync(dto.RoleId, ct);

        if (role is null)
        {
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.Role.RoleNotFound;
        }

        var user = await _usersRepository.GetByIdAsync(dto.UserId, ct);

        if (user is null)
        {
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.User.NotFound;
        }
        
        user.AddRole(dto.RoleId);

        if (role.Requirements.Count > 0)
        {
            var providedKeys = dto.Attributes?.Select(a => a.Key).ToHashSet() ?? [];
            
            foreach (var requirement in role.Requirements)
            {
                if (!providedKeys.Contains(requirement.ValueName))
                {
                    await _uow.RollbackTransactionAsync(ct);
                    return ApplicationErrors.Role.RoleRequirementMissing;
                }
            }
        }
        
        var roleAlreadyGranted = user.RoleIds.Any(ur => ur == dto.RoleId);
        
        if (!roleAlreadyGranted)
        {
            user.AddRole(dto.RoleId);
        }

        if (dto.Attributes is not null)
        {
            foreach (var attrDto in dto.Attributes)
            {
                user.AddOrUpdateAttribute(attrDto.Key, attrDto.Value);
            }
        }
        
        await _uow.SaveChangesAsync(ct);
        await _uow.CommitTransactionAsync(ct);

        return UnitResult.Success<Error>();
    }

    public Task<List<RoleDto>> GetRolesAsync(CancellationToken ct)
    {
        return _rolesRepository.GetAllDtoAsync(ct);
    }
}