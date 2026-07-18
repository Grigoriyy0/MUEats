using Ardalis.Specification;
using MUEats.Discounts.Core.Discount;

namespace MUEats.Discounts.Application.Specifications;

public class GetRestaurantDiscountsSpecification : Specification<Discount>
{
    public GetRestaurantDiscountsSpecification(Guid restaurantId)
    {
        Query.Where(x => x.RestaurantId == restaurantId);
    }
}