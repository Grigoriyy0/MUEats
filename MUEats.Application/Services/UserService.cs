using MUEats.Application.Dto.User;
using MUEats.Application.Ports;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Utils;

namespace MUEats.Application.Services;

public class UserService(
    IUsersRepository usersRepository,
    IHashProvider hashProvider,
    IUnitOfWork uow,
    ITokenProducer tokenProducer
    )
{
    public async Task CreateAsync(CreateUserDto dto, CancellationToken ct)
    {
        try
        {
            await uow.BeginTransactionAsync(ct);

            var checkUser = await usersRepository.AnyAsync(dto.Email, ct);

            if (checkUser)
            {
                throw new ArgumentException("User with that email already exists");
            }
            
            //todo password validation 
            
            var passwordHash = hashProvider.ComputeHash(dto.Password);
        
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Username = dto.Username,
                DefaultAddress = dto.DefaultAddress,
                PasswordHash = passwordHash,
                Role = Role.Customer
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

    public async Task<string> AuthAsync(AuthDto dto, CancellationToken ct)
    {
        var user = await usersRepository.GetByEmailAsync(dto.Email, ct);

        if (user is null)
        {
            throw new ArgumentException("Login or password is incorrect");
        }
        
        var passwordHash = hashProvider.ComputeHash(dto.Password);

        if (user.PasswordHash != passwordHash)
        {
            throw new ArgumentException("Login or password is incorrect");
        }

        var token = tokenProducer.ProduceToken(user);

        return token;
    }
}