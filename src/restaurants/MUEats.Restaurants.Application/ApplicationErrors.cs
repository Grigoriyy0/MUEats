using Primitives;

namespace MUEats.Restaurants.Application;

public static class ApplicationErrors
{
    public static class Restaurant
    {
        public static readonly Error NotFound = GeneralError.ValueIsIncorrect("restaurant.id");
        
        public static readonly Error AlreadyExists = GeneralError.ValueIsIncorrect("restaurant.name");
    }

    public static class Menu
    {
        public static readonly Error NotFound = GeneralError.ValueIsIncorrect("menu.id");
        
        public static readonly Error AlreadyExists = GeneralError.ValueIsIncorrect("menu.restaurantId");
    }

    public static class OrderSnapshot
    {
        public static readonly Error NotFound = GeneralError.ValueIsIncorrect("orderSnapshot.id");

        public static readonly Error WrongRestaurant = GeneralError.ValueIsIncorrect("order.restaurantId");
    }
}