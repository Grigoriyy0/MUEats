using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MUEats.Identity.Application.Ports;
using MUEats.Identity.Core.Domain.Constants;
using MUEats.Identity.Core.Domain.Role;
using MUEats.Identity.Core.Domain.User;
using MUEats.Identity.Infrastructure.Options;
using MUEats.Identity.Infrastructure.Persistence.DbContexts;
using MUEats.Identity.Infrastructure.Utils;

namespace MUEats.Identity.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly AdminOptions _adminOptions;
    private readonly IdentityDbContext _dbContext;
    private readonly IHashProvider _hashProvider;

    public DatabaseSeeder(
        IdentityDbContext dbContext,
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
        var existingRoles = await _dbContext.Roles
            .Include(r => r.Requirements)
            .ToDictionaryAsync(r => r.Id, ct);

        var rolesToSeed = new List<Role>();

        if (!existingRoles.ContainsKey(RoleConstants.RoleIds.User))
        {
            var userRole = Role.Create(RoleConstants.RoleNames.User).Value;

            userRole.SetProperty("Id", RoleConstants.RoleIds.User);
            
            rolesToSeed.Add(userRole);
        }
        
        if (!existingRoles.ContainsKey(RoleConstants.RoleIds.Admin))
        {
            var adminRole = Role.Create(RoleConstants.RoleNames.Admin).Value;

            adminRole.SetProperty("Id", RoleConstants.RoleIds.Admin);
            
            rolesToSeed.Add(adminRole);
        }

        if (!existingRoles.TryGetValue(RoleConstants.RoleIds.RestaurantOwner, out var restaurantOwnerRole))
        {
            var newRole = Role.Create(RoleConstants.RoleNames.RestaurantOwner).Value;

            newRole.SetProperty("Id", RoleConstants.RoleIds.RestaurantOwner);
            
            newRole.AddOrUpdateRequirement("restaurant_id");
            rolesToSeed.Add(newRole);
        }
        else
        {
            if (restaurantOwnerRole.Requirements.All(r => r.ValueName != "restaurant_id"))
            {
                restaurantOwnerRole.AddOrUpdateRequirement("restaurant_id");
            }
        }

        if (rolesToSeed.Count > 0)
        {
            await _dbContext.Roles.AddRangeAsync(rolesToSeed, ct);
        }

        await _dbContext.SaveChangesAsync(ct);
    }

    private async Task SeedAdminUserAsync(CancellationToken ct)
    {
        var adminExists = await _dbContext.Users
            .AnyAsync(u => u.EmailAddress.Value == _adminOptions.Email, ct);

        if (adminExists) return;

        var passwordHash = _hashProvider.ComputeHash(_adminOptions.Password);

        var adminUser = User.Create(
            firstName: _adminOptions.FirstName,
            lastName: _adminOptions.LastName,
            emailAddress: _adminOptions.Email,
            passwordHash: passwordHash
        ).Value;

        adminUser.AddRole(RoleConstants.RoleIds.Admin);

        await _dbContext.Users.AddAsync(adminUser, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}