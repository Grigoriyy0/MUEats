using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MUEats.Application.Ports;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;
using MUEats.Infrastructure.Options;

namespace MUEats.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly MueDbContext _dbContext;
    private readonly AdminOptions _adminOptions;
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

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);

        await SeedAdminUserAsync(cancellationToken);
    }

    private async Task SeedAdminUserAsync(CancellationToken cancellationToken)
    { 
        var adminExists = await _dbContext.Users
            .AnyAsync(u => u.Email == _adminOptions.Email, cancellationToken);

        if (adminExists)
        {
            return; 
        }

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username =  _adminOptions.UserName,
            FirstName = _adminOptions.FirstName,
            LastName = _adminOptions.LastName,
            Email = _adminOptions.Email,
            Role = Role.Admin,
            PasswordHash = _hashProvider.ComputeHash(_adminOptions.Password)
        };

        await _dbContext.Users.AddAsync(adminUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}