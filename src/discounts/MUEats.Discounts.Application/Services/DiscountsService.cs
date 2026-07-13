using CSharpFunctionalExtensions;
using MUEats.Discounts.Application.Dtos;
using MUEats.Discounts.Application.Ports;
using MUEats.Discounts.Application.Queries;
using MUEats.Discounts.Application.Specifications;
using MUEats.Discounts.Core.Discount;
using Primitives;

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
    
    public async Task<UnitResult<Error>> CreateAsync(CreateDiscountDto dto, CancellationToken ct)
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
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.Discount.AlreadyExists;
        }
        
        var discountResult = Discount.Create(dto.RestaurantId, 
            dto.FoodItemId, 
            dto.Value, 
            dto.TargetRoles, 
            dto.Type, 
            dto.ActiveFrom, 
            dto.ActiveBefore, 
            dto.MaxUsageCount, 
            dto.MinOrderValue);
        
        if (discountResult.IsFailure)
        {
            await _uow.RollbackTransactionAsync(ct);
            return discountResult.Error;
        }

        var discount = discountResult.Value;
        
        await _discounts.AddAsync(discount, ct);

        await _uow.SaveChangesAsync(ct);
        await _uow.CommitTransactionAsync(ct);

        return UnitResult.Success<Error>();
    }

    public async Task<List<DiscountDto>> GetByFilterAsync(GetDiscountsQuery query, CancellationToken ct)
    {
        var spec = new GetApplicableDiscountsSpecification(query.RestaurantId, 
            query.FoodItemIds, 
            query.CartSubtotal,
            query.Roles);

        var discounts = await _discounts.ListAsync(spec, ct);

        return discounts.Select(x => new DiscountDto
        {
            Id = x.Id,
            Type = x.Type.ToString(),
            FoodItemId = x.FoodItemId,
            Value = x.Value
        }).ToList();
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