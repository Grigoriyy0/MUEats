using Primitives;

namespace MUEats.Restaurants.Application;

public static class ApplicationErrors
{
    public static class Restaurant
    {
        public static Error RestaurantNotFound = GeneralError.ValueIsIncorrect("restaurant.id");
        
        public static Error RestaurantAlreadyExists = GeneralError.ValueIsIncorrect("restaurant.name");
    }

    public static class Menu
    {
        public static Error MenuNotFound = GeneralError.ValueIsIncorrect("menu.id");
    }
}