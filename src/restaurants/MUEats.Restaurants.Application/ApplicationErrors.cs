using Primitives;

namespace MUEats.Restaurants.Application;

public static class ApplicationErrors
{
    public static class Restaurant
    {
        public static readonly Error RestaurantNotFound = GeneralError.ValueIsIncorrect("restaurant.id");
        
        public static readonly Error RestaurantAlreadyExists = GeneralError.ValueIsIncorrect("restaurant.name");
    }

    public static class Menu
    {
        public static readonly Error MenuNotFound = GeneralError.ValueIsIncorrect("menu.id");
        
        public static readonly Error MenuAlreadyExists = GeneralError.ValueIsIncorrect("menu.restaurantId");
    }
}