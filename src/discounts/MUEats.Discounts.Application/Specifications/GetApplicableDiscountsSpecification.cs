using Ardalis.Specification;
using MUEats.Discounts.Core.Discount;

namespace MUEats.Discounts.Application.Specifications;

public class GetApplicableDiscountsSpecification : Specification<Discount>
{
    public GetApplicableDiscountsSpecification(Guid restaurantId, 
        List<Guid> foodItems, 
        decimal cartSubTotal, 
        List<string> roles)
    {
        Query.AsNoTracking()
            .Where(x => x.RestaurantId == restaurantId && x.IsActive)
            .Where(x => x.ActiveBefore > DateTime.UtcNow && x.ActiveFrom < DateTime.UtcNow)
            .Where(d => d.TargetedRoles.Any(z => roles.Contains(z.RoleName)))
            .Where(d => d.MinOrderValue == null || d.MinOrderValue <= cartSubTotal);


        if (foodItems.Count > 0)
        {
            Query.Where(d => d.FoodItemId == null || foodItems.Contains(d.FoodItemId.Value));
        }
        else
        {
            Query.Where(d => d.FoodItemId == null);
        }
    }
}