using MUEats.Application.Dto.User;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Application.Queries;
using MUEats.Core.Domain.Constants;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _repository;
    private readonly IHashProvider _hashProvider;
    private readonly IUnitOfWork _uow;

    public UsersService(IUsersRepository repository, IHashProvider hashProvider, IUnitOfWork uow)
    {
        _repository = repository;
        _hashProvider = hashProvider;
        _uow = uow;
    }

    public async Task CreateAsync(CreateUserDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
        {
            //todo error
            return;
        }

        var userExists = await _repository.AnyAsync(dto.Email, ct);

        if (userExists)
        {
            //todo error
            return;
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
    }
    
    public Task<List<UserDto>> GetFilteredAsync(GetUsersQuery query, CancellationToken ct)
    {
        query.Page = query.Page <= 0 ? 1 : query.Page;
        query.PageSize = query.PageSize <= 0 ? 20 : query.PageSize;

        return _repository.GetUsersAsync(query, ct);
    }
}