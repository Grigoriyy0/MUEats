using MUEats.Discounts.Application.Dtos;
using MUEats.Discounts.Application.Ports;
using MUEats.Discounts.Application.Specifications;

namespace MUEats.Discounts.Application.Services;

public class DiscountsService
{
    private readonly IUnitOfWork _uow;
    private readonly IDiscountsRepository _discounts;

    public DiscountsService(IUnitOfWork uow, IDiscountsRepository discounts)
    {
        _uow = uow;
        _discounts = discounts;
    }
    
    public async Task CreateAsync(CreateDiscountDto dto, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);

        var spec = new GetDiscountsByRestaurantSpecification(dto.RestaurantId, dto.FoodItemId, dto.Type, dto.Value);

        var existingDiscounts = await _discounts.ListAsync(spec, ct);

        dto.TargetRoles = dto.TargetRoles.OrderBy(r => r).ToList();

        var rolesDuplicated = existingDiscounts.Any(d =>
        {
            var roles = d.TargetedRoles
                .Select(r => r.RoleName)
                .OrderBy(r => r)
                .ToList();

            return roles.SequenceEqual(dto.TargetRoles);
        });

        if (rolesDuplicated)
        {
            //todo error
            return;
        }
    }

    public async Task DeleteAsync(Guid discountId, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);

        var discount = await _discounts.GetByIdAsync(discountId, ct);

        if (discount is null)
        {
            await _uow.RollbackTransactionAsync(ct);
            return;
            // todo error
        }

        await _discounts.DeleteAsync(discount, ct);

        await _uow.SaveChangesAsync(ct);
        await _uow.CommitTransactionAsync(ct);
    }
}