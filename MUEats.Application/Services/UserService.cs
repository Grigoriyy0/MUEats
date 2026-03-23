using MUEats.Application.Dto.User;
using MUEats.Application.Ports;
using MUEats.Application.Responses;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;
using MUEats.Core.Domain.User.Utils;

namespace MUEats.Application.Services;

public class UserService(
    IUsersRepository usersRepository,
    IHashProvider hashProvider,
    IUnitOfWork uow,
    ITokenProducer tokenProducer,
    IRefreshTokenService refreshTokenService,
    IPasswordValidator passwordValidator
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
            
            var validationResult = passwordValidator.Validate(dto.Password);

            if (!validationResult)
            {
                throw new ArgumentException("Password does not match requirements");
            }
            
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

    public async Task<TokenResponse> AuthAsync(AuthDto dto, CancellationToken ct)
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

        var tokenResponse = tokenProducer.ProduceTokenPair(user);

        await refreshTokenService.SaveAsync(user.Id, tokenResponse.RefreshToken, ct);

        return tokenResponse;
    }

    public async Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var token = await refreshTokenService.GetAsync(refreshToken, ct);

        if (token is null)
        {
            throw new ArgumentException("No such token");
        }

        var user = await usersRepository.GetByIdAsync(token.UserId, ct);

        if (user is null)
        {
            throw new ArgumentException("No such user");
        }

        var newTokenPair = tokenProducer.ProduceTokenPair(user);

        await refreshTokenService.SaveAsync(user.Id, newTokenPair.RefreshToken, ct);
        
        return newTokenPair;
    }
}