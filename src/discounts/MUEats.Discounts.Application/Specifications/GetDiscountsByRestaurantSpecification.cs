using Ardalis.Specification;
using MUEats.Discounts.Core.Discount;

namespace MUEats.Discounts.Application.Specifications;

public class GetDiscountsByRestaurantSpecification : Specification<Discount>
{
    public GetDiscountsByRestaurantSpecification(Guid restaurantId, Guid? foodItemId, DiscountType discountType, decimal value)
    {
        Query.Where(x => x.RestaurantId == restaurantId && 
                         x.FoodItemId == foodItemId && 
                         x.Type == discountType && 
                         x.Value == value)
            .Include(y => y.TargetedRoles);
    }
}