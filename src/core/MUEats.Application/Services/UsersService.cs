using CSharpFunctionalExtensions;
using MUEats.Application.Dto.User;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Application.Queries;
using MUEats.Core.Domain.Constants;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;
using Primitives;

namespace MUEats.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _repository;
    private readonly IHashProvider _hashProvider;

    public UsersService(IUsersRepository repository, IHashProvider hashProvider)
    {
        _repository = repository;
        _hashProvider = hashProvider;
    }

    public async Task<UnitResult<Error>> CreateAsync(CreateUserDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
        {
            return ApplicationErrors.User.NameIsEmpty;
        }

        var userExists = await _repository.AnyAsync(dto.Email, ct);

        if (userExists)
        {
            return ApplicationErrors.User.UserAlreadyExists;
        }

        var passwordHash = _hashProvider.ComputeHash(dto.Password);
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PasswordHash = passwordHash,
            UserRoles = [new UserRole
            {
                RoleId = RoleConstants.RoleIds.User,
                UserId = Guid.NewGuid()
            }]
        };

        await _repository.AddAsync(user, ct);

        return UnitResult.Success<Error>();
    }
    
    public Task<List<UserDto>> GetFilteredAsync(GetUsersQuery query, CancellationToken ct)
    {
        query.Page = query.Page <= 0 ? 1 : query.Page;
        query.PageSize = query.PageSize <= 0 ? 20 : query.PageSize;

        return _repository.GetUsersAsync(query, ct);
    }
}