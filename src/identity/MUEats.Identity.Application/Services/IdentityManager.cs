using CSharpFunctionalExtensions;
using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Application.Interfaces;
using MUEats.Identity.Application.Ports;
using MUEats.Identity.Application.Responses;
using MUEats.Identity.Core.Domain.User;
using Primitives;

namespace MUEats.Identity.Application.Services;

public class IdentityManager : IIdentityManager
{
    private readonly IUsersRepository _usersRepository;
    private readonly IRolesRepository _rolesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenProducer _tokenProducer;
    private readonly IHashProvider _hashProvider;

    public IdentityManager(IUsersRepository usersRepository, 
        IRolesRepository rolesRepository, 
        IUnitOfWork unitOfWork, 
        ITokenProducer tokenProducer, 
        IHashProvider hashProvider)
    {
        _usersRepository = usersRepository;
        _rolesRepository = rolesRepository;
        _unitOfWork = unitOfWork;
        _tokenProducer = tokenProducer;
        _hashProvider = hashProvider;
    }

    public async Task<UnitResult<Error>> SignupAsync(SignupDto dto, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        
        try
        {
            var userExists = await _usersRepository.AnyAsync(dto.Email, ct);

            if (userExists)
            {
                return ApplicationErrors.User.UserAlreadyExists;
            }

            var passwordHash = _hashProvider.ComputeHash(dto.Password);

            var userResult = User.Create(dto.FirstName, dto.LastName, dto.Email, passwordHash);

            if (userResult.IsFailure)
            {
                return userResult.Error;
            }

            await _usersRepository.AddAsync(userResult.Value, ct);

            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);
            
            return UnitResult.Success<Error>();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<Result<AuthResponse, Error>> SigninAsync(SigninDto dto, CancellationToken ct)
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

        var roles = await _rolesRepository.GetRolesByIdsAsync(user.RoleIds.ToList(), ct);

        var token = _tokenProducer.ProduceToken(user, roles);

        return new AuthResponse
        {
            AccessToken = token
        };
    }
}