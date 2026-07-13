using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Discounts.Core.Discount;

public class Discount : Entity<Guid>
{
    private Discount() { }

    private Discount(Guid restaurantId, 
        Guid? foodItemId, 
        decimal value, 
        DiscountType type, 
        DateTime activeFrom, 
        DateTime? activeBefore, 
        int? maxUsageCount, 
        decimal? minOrderValue)
    {
        RestaurantId = restaurantId;
        FoodItemId = foodItemId;
        Value = value;
        Type = type;
        ActiveFrom = activeFrom;
        ActiveBefore = activeBefore;
        IsActive = false;
        MaxUsageCount = maxUsageCount;
        MinOrderValue = minOrderValue;
    }

    public Guid RestaurantId { get; init; }
    
    public Guid? FoodItemId { get; init; }
    
    public decimal Value { get; private set; }

    public DiscountType Type { get; init; }

    private List<IdentityRole> _targetedRoles = [];

    public IReadOnlyList<IdentityRole> TargetedRoles => _targetedRoles.AsReadOnly();
    
    public DateTime ActiveFrom { get; init; }
    
    public DateTime? ActiveBefore { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public int? MaxUsageCount { get; init; }
    
    public decimal? MinOrderValue { get; private set; }

    public static Result<Discount, Error> Create(
        Guid restaurantId,
        Guid? foodItemId,
        decimal value,
        List<string> targetRoles,
        DiscountType type,
        DateTime activeFrom,
        DateTime? activeBefore,
        int? maxUsageCount,
        decimal? minOrderValue)
    {
        if (value <= 0 && type == DiscountType.Fixed)
        {
            return DomainErrors.Discount.InvalidFixedValue;
        }

        if ((value <= 0 || value >= 100) && type == DiscountType.Percentage)
        {
            return DomainErrors.Discount.InvalidPercentageValue;
        }

        if (targetRoles.Count == 0)
        {
            return DomainErrors.Discount.TargetRolesRequired;
        }
        
        if (activeBefore is not null && activeBefore <= activeFrom)
        {
            return DomainErrors.Discount.InvalidExpirationDate;
        }

        if (maxUsageCount is not null && maxUsageCount <= 0)
        {
            return DomainErrors.Discount.InvalidMaxUsageCount;
        }
        
        if (minOrderValue is not null && minOrderValue < 0)
        {
            return DomainErrors.Discount.NegativeMinOrderValue;
        }
        
        var discount = new Discount(
            restaurantId, 
            foodItemId, 
            value, 
            type, 
            activeFrom, 
            activeBefore, 
            maxUsageCount, 
            minOrderValue);
        
        foreach (var roleName in targetRoles)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return DomainErrors.Discount.TargetRoleName;
            }

            var role = new IdentityRole
            {
                Id = Guid.NewGuid(),
                RoleName = roleName
            };
            
            discount._targetedRoles.Add(role);
        }

        return discount;
    }
}