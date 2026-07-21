using CSharpFunctionalExtensions;
using MUEats.Identity.Application.Dtos;
using Primitives;

namespace MUEats.Identity.Application.Interfaces;

public interface IRolesService
{
    Task<UnitResult<Error>> CreateAsync(CreateRoleDto dto, CancellationToken ct);
    
    Task<UnitResult<Error>> GrantRoleAsync(GrantRoleDto dto, CancellationToken ct);
    
    Task<List<RoleDto>> GetRolesAsync(CancellationToken ct);
}