using MUEats.Application.Dto.User;
using MUEats.Application.Ports;
using MUEats.Application.Responses;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Application.Services;

public class UserService(IUsersRepository usersRepository,
    IHashProvider hashProvider,
    IUnitOfWork uow)
{
    public async Task CreateManagerAsync(CreateManagerDto dto, CancellationToken ct)
    {
        try
        {
            await uow.BeginTransactionAsync(ct);
            
            var passwordHash = hashProvider.ComputeHash(dto.Password);

            var attribute = new UserAttribute
            {
                Key = "restaurant_id",
                Value = dto.RestaurantId.ToString()
            };
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.UserName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Role = Role.RestaurantManager,
                PasswordHash = passwordHash,
                UserAttributes = [attribute]
            };
            
            await usersRepository.AddAsync(user, ct);
        
            await uow.SaveChangesAsync(ct);
            await uow.CommitTransactionAsync(ct);
        }
        catch (Exception)
        {
            await uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}