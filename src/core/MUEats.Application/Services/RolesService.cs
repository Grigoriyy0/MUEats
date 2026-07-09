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

    public async Task<UnitResult<Error>> CreateAsync(CreateRoleDto dto, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);

        var roleExists = await _rolesRepository.AnyAsync(dto.RoleName, ct);

        if (roleExists)
        {
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.Role.RoleAlreadyExists;
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            RoleName = dto.RoleName
        };

        if (dto.RequiredAttributes is not null)
        {
            foreach (var attribute in dto.RequiredAttributes)
            {
                role.Requirements.Add(new RoleRequirement
                {
                    Id = Guid.NewGuid(),
                    ValueName = attribute,
                    RoleId = role.Id
                });
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

        user.UserRoles.Add(new UserRole
        {
            RoleId = dto.RoleId,
            UserId = dto.UserId
        });

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
        
        var roleAlreadyGranted = user.UserRoles.Any(ur => ur.RoleId == dto.RoleId);
        
        if (!roleAlreadyGranted)
        {
            user.UserRoles.Add(new UserRole
            {
                RoleId = dto.RoleId,
                UserId = dto.UserId
            });
        }

        if (dto.Attributes is not null)
        {
            foreach (var attrDto in dto.Attributes)
            {
                var existingAttribute = user.UserAttributes.FirstOrDefault(ua => ua.Key == attrDto.Key);
                
                if (existingAttribute is not null)
                {
                    existingAttribute.Value = attrDto.Value;
                }
                else
                {
                    user.UserAttributes.Add(new UserAttribute
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Key = attrDto.Key,
                        Value = attrDto.Value
                    });
                }
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