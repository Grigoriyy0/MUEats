using MUEats.Application.Dto.User;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Application.Responses;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Application.Services.Identity;

public class AuthService : IAuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IHashProvider _hashProvider;
    private readonly ITokenProducer _tokenProducer;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordValidator _passwordValidator;
    
    public AuthService(IRefreshTokenService refreshTokenService, 
        ITokenProducer tokenProducer, 
        IHashProvider hashProvider, 
        IUsersRepository usersRepository, 
        IUnitOfWork uow, 
        IPasswordValidator passwordValidator)
    {
        _refreshTokenService = refreshTokenService;
        _tokenProducer = tokenProducer;
        _hashProvider = hashProvider;
        _usersRepository = usersRepository;
        _uow = uow;
        _passwordValidator = passwordValidator;
    }

    public async Task<TokenResponse> AuthAsync(AuthDto dto, CancellationToken ct)
    {
        var user = await _usersRepository.GetByEmailAsync(dto.Email, ct);

        if (user is null)
        {
            throw new ArgumentException("Login or password is incorrect");
        }
        
        var verifyResult = _hashProvider.VerifyHash(dto.Password, user.PasswordHash);
        
        if (!verifyResult)
        {
            throw new ArgumentException("Login or password is incorrect");
        }

        var tokenResponse = _tokenProducer.ProduceTokenPair(user);

        await _refreshTokenService.SaveAsync(user.Id, tokenResponse.RefreshToken, ct);

        return tokenResponse;
    }
    
    public async Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var token = await _refreshTokenService.GetAsync(refreshToken, ct);

        if (token is null)
        {
            throw new ArgumentException("No such token");
        }

        var user = await _usersRepository.GetByIdAsync(token.UserId, ct);

        if (user is null)
        {
            throw new ArgumentException("No such user");
        }

        var newTokenPair = _tokenProducer.ProduceTokenPair(user);

        await _refreshTokenService.SaveAsync(user.Id, newTokenPair.RefreshToken, ct);
        
        return newTokenPair;
    }
    
    public async Task RegisterAsync(CreateUserDto dto, CancellationToken ct)
    {
        try
        {
            await _uow.BeginTransactionAsync(ct);

            var checkUser = await _usersRepository.AnyAsync(dto.Email, ct);

            if (checkUser)
            {
                throw new ArgumentException("User with that email already exists");
            }
            
            var validationResult = _passwordValidator.Validate(dto.Password);

            if (!validationResult)
            {
                throw new ArgumentException("Password does not match requirements");
            }
            
            var passwordHash = _hashProvider.ComputeHash(dto.Password);
        
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = passwordHash,
                Role = Role.Customer
            };

            await _usersRepository.AddAsync(user, ct);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
        }
        catch (Exception)
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}