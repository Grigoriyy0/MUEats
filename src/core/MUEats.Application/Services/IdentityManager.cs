using CSharpFunctionalExtensions;
using MUEats.Application.Dto.User;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Application.Responses;
using Primitives;

namespace MUEats.Application.Services;

public class IdentityManager : IIdentityManager
{
    private readonly IUsersRepository _usersRepository;
    private readonly IHashProvider _hashProvider;
    private readonly IUnitOfWork _uow;
    private readonly ITokenProducer _tokenProducer;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IPasswordValidator _passwordValidator;
    private readonly IUsersService _usersService;

    public IdentityManager(IUsersRepository usersRepository, 
        IHashProvider hashProvider, 
        IUnitOfWork uow, 
        ITokenProducer tokenProducer, 
        IRefreshTokenService refreshTokenService, 
        IPasswordValidator passwordValidator, 
        IUsersService usersService)
    {
        _usersRepository = usersRepository;
        _hashProvider = hashProvider;
        _uow = uow;
        _tokenProducer = tokenProducer;
        _refreshTokenService = refreshTokenService;
        _passwordValidator = passwordValidator;
        _usersService = usersService;
    }

    public async Task<Result<TokenResponse, Error>> AuthAsync(AuthDto dto, CancellationToken ct)
    {
        var user = await _usersRepository.GetByEmailAsync(dto.Email, ct);

        if (user is null)
        {
            return ApplicationErrors.User.InvalidDetails;
        }
        
        var verifyResult = _hashProvider.VerifyHash(dto.Password, user.PasswordHash);
        
        if (!verifyResult)
        {
            return ApplicationErrors.User.InvalidDetails;
        }

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var tokenResponse = _tokenProducer.ProduceTokenPair(user);
            await _refreshTokenService.SaveAsync(user.Id, tokenResponse.RefreshToken, ct);
            
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
            
            return tokenResponse;
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
    
    public async Task<Result<TokenResponse, Error>> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var token = await _refreshTokenService.GetAsync(refreshToken, ct);

            if (token is null)
            {
                await _uow.RollbackTransactionAsync(ct);
                return ApplicationErrors.User.NotFound;
            }

            if (token.IsRevoked)
            {
                await _refreshTokenService.RevokeAllForUserAsync(token.UserId, ct);
                await _uow.SaveChangesAsync(ct);
                await _uow.CommitTransactionAsync(ct);
                
                return ApplicationErrors.User.InvalidDetails; 
            }

            if (token.ExpiresOn <= DateTime.UtcNow)
            {
                await _uow.RollbackTransactionAsync(ct);
                return ApplicationErrors.User.NotFound; 
            }

            var user = await _usersRepository.GetByIdAsync(token.UserId, ct);
                
            if (user is null)
            {
                await _uow.RollbackTransactionAsync(ct);
                return ApplicationErrors.User.NotFound;
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
    
    public async Task<UnitResult<Error>> RegisterAsync(CreateUserDto dto, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            if (dto.Password != dto.PasswordConfirmation)
            {
                await _uow.RollbackTransactionAsync(ct);
                return ApplicationErrors.User.PasswordsDoNotMatch;
            }
                
            var validationResult = _passwordValidator.Validate(dto.Password);

            if (!validationResult)
            {
                await _uow.RollbackTransactionAsync(ct);
                return ApplicationErrors.User.PasswordDoesNotMatchRequirements;
            }
            
            var userResult = await _usersService.CreateAsync(dto, ct);

            if (userResult.IsFailure)
            {
                await _uow.RollbackTransactionAsync(ct);
                return userResult;
            }
            
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            return UnitResult.Success<Error>();
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}