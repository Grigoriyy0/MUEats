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
        var existingRoleIds = await _dbContext.Roles
            .Select(r => r.Id)
            .ToListAsync(ct);

        var rolesToSeed = new List<Role>();

        if (!existingRoleIds.Contains(RoleConstants.RoleIds.User))
        {
            rolesToSeed.Add(new Role
            {
                Id = RoleConstants.RoleIds.User,
                RoleName = RoleConstants.RoleNames.User
            });
        }

        if (!existingRoleIds.Contains(RoleConstants.RoleIds.Admin))
        {
            rolesToSeed.Add(new Role
            {
                Id = RoleConstants.RoleIds.Admin,
                RoleName = RoleConstants.RoleNames.Admin
            });
        }

        if (!existingRoleIds.Contains(RoleConstants.RoleIds.RestaurantOwner))
        {
            rolesToSeed.Add(new Role
            {
                Id = RoleConstants.RoleIds.RestaurantOwner,
                RoleName = RoleConstants.RoleNames.RestaurantOwner,
                Requirements = new List<RoleRequirement>
                {
                    new RoleRequirement
                    {
                        Id = Guid.NewGuid(),
                        ValueName = "restaurant_id"
                    }
                }
            });
        }
        else
        {
            var restaurantOwnerRole = await _dbContext.Roles
                .Include(x => x.Requirements)
                .FirstAsync(x => x.Id == RoleConstants.RoleIds.RestaurantOwner, ct);

            if (restaurantOwnerRole.Requirements.All(r => r.ValueName != "restaurant_id"))
            {
                restaurantOwnerRole.Requirements.Add(new RoleRequirement
                {
                    Id = Guid.NewGuid(),
                    ValueName = "restaurant_id"
                });
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
            .AnyAsync(u => u.Email == _adminOptions.Email, ct);

        if (adminExists) return;
        
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
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