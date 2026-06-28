using System.Security;
using MUEats.Application.Dto.User;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Application.Responses;

namespace MUEats.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IHashProvider _hashProvider;
    private readonly ITokenProducer _tokenProducer;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordValidator _passwordValidator;
    private readonly IUsersService _usersService;
    
    public AuthService(IRefreshTokenService refreshTokenService, 
        ITokenProducer tokenProducer, 
        IHashProvider hashProvider, 
        IUsersRepository usersRepository, 
        IUnitOfWork uow, 
        IPasswordValidator passwordValidator, 
        IUsersService usersService)
    {
        _refreshTokenService = refreshTokenService;
        _tokenProducer = tokenProducer;
        _hashProvider = hashProvider;
        _usersRepository = usersRepository;
        _uow = uow;
        _passwordValidator = passwordValidator;
        _usersService = usersService;
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
        await _uow.BeginTransactionAsync(ct);
        try 
        {
            var token = await _refreshTokenService.GetAsync(refreshToken, ct);

            if (token is null)
            {
                throw new Exception("Token not found");
            }

            if (token.IsRevoked)
            {
                await _refreshTokenService.RevokeAllForUserAsync(token.UserId, ct);
                await _uow.SaveChangesAsync(ct);
                await _uow.CommitTransactionAsync(ct);
                throw new SecurityException("Token reuse detected. All sessions revoked.");
            }

            if (token.ExpiresOn <= DateTime.UtcNow)
                throw new Exception("Token expired");

            var user = await _usersRepository.GetByIdAsync(token.UserId, ct);
            
            if (user is null)
            {
                throw new Exception("User unavailable");
            }

            var newTokenPair = _tokenProducer.ProduceTokenPair(user);
            
            token.IsRevoked = true;
            await _refreshTokenService.SaveAsync(user.Id, newTokenPair.RefreshToken, ct);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
        
            return newTokenPair;
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
    
    public async Task RegisterAsync(CreateUserDto dto, CancellationToken ct)
    {
        try
        {
            await _uow.BeginTransactionAsync(ct);

            if (dto.Password != dto.PasswordConfirmation)
            {
                throw new ArgumentException("Passwords do not match");
            }
            
            var validationResult = _passwordValidator.Validate(dto.Password);

            if (!validationResult)
            {
                throw new ArgumentException("Password does not match requirements");
            }


            await _usersService.CreateAsync(dto, ct);
            
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