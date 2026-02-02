using MUEats.Application.Dto.Restaurant;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Restaurant;

namespace MUEats.Application.Services;

public class RestaurantsService(IRestaurantsRepository repository, IUnitOfWork uow)
{
    public async Task CreateAsync(CreateRestaurantDto dto, CancellationToken ct)
    {
        await uow.BeginTransactionAsync(ct);

        if (dto.OpeningHours == TimeSpan.Zero || dto.ClosingHours == TimeSpan.Zero)
        {
            throw new Exception("Opening hours and closing hours are required");
        }

        if (dto.OpeningHours > dto.ClosingHours)
        {
            throw new Exception("Opening hours can not be greater than closing hours");
        }
        
        var restaurant = new Restaurant
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Address = dto.Address,
            OpeningHours = dto.OpeningHours,
            ClosingHours = dto.ClosingHours,
        };
        
        await repository.AddAsync(restaurant, ct);
        
        await uow.SaveChangesAsync(ct);
        await uow.CommitTransactionAsync(ct);
    }
    
}