using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Constants;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;
using MUEats.Infrastructure.Options;

namespace MUEats.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly AdminOptions _adminOptions;
    private readonly MueDbContext _dbContext;
    private readonly IHashProvider _hashProvider;

    public DatabaseSeeder(
        MueDbContext dbContext,
        IOptions<AdminOptions> adminOptions,
        IHashProvider hashProvider)
    {
        _dbContext = dbContext;
        _adminOptions = adminOptions.Value;
        _hashProvider = hashProvider;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await _dbContext.Database.MigrateAsync(ct);

        await SeedRolesAsync(ct);
        await SeedAdminUserAsync(ct);
    }

    private async Task SeedRolesAsync(CancellationToken ct)
    {
        var roles = new Dictionary<Guid, string>
        {
            [RoleConstants.RoleIds.User] = RoleConstants.RoleNames.User,
            [RoleConstants.RoleIds.Admin] = RoleConstants.RoleNames.Admin,
            [RoleConstants.RoleIds.RestaurantOwner] = RoleConstants.RoleNames.RestaurantOwner
        };

        var existingRoles = await _dbContext.Roles
            .Select(r => r.RoleName)
            .ToListAsync(ct);

        var newRoles = roles
            .Where(r => !existingRoles.Contains(r.Value))
            .Select(r => new Role
            {
                Id = r.Key,
                RoleName = r.Value
            });

        await _dbContext.Roles.AddRangeAsync(newRoles, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    private async Task SeedAdminUserAsync(CancellationToken ct)
    {
        var adminExists = await _dbContext.Users
            .AnyAsync(u => u.Email == _adminOptions.Email, ct);

        if (adminExists) return;
        
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username = _adminOptions.UserName,
            FirstName = _adminOptions.FirstName,
            LastName = _adminOptions.LastName,
            Email = _adminOptions.Email,
            PasswordHash = _hashProvider.ComputeHash(_adminOptions.Password)
        };
        
        adminUser.UserRoles.Add(new UserRole
        {
            UserId = adminUser.Id,
            RoleId = RoleConstants.RoleIds.Admin
        });

        await _dbContext.Users.AddAsync(adminUser, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}