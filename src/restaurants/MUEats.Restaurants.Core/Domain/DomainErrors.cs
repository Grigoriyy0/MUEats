using Primitives;

namespace MUEats.Restaurants.Core.Domain;

public static class DomainErrors
{
    public static class Restaurant
    {
        public static readonly Error NameIsEmpty = GeneralError.ValueIsIncorrect("restaurant.name");
        
        public static readonly Error AddressIsEmpty = GeneralError.ValueIsIncorrect("restaurant.address");
    }

    public static class RestaurantBusinessHours
    {
        public static readonly Error EndIsEarlier = GeneralError.ValueIsIncorrect("businessHours.endTime");
    }
    
    public static class Menu
    {
        public static readonly Error CategoryAlreadyExists = GeneralError.ValueIsIncorrect("menu.category");
        
        public static readonly Error MenuItemAlreadyExists = GeneralError.ValueIsIncorrect("menu.item");
        
        public static readonly Error MenuItemDoesNotExist = GeneralError.ValueIsIncorrect("menu.itemId");
        
        public static readonly Error CategoryDoesNotExists  = GeneralError.ValueIsIncorrect("menu.category");
    }

    public static class MenuItem
    {
        public static readonly Error ItemNameIsEmpty = GeneralError.ValueIsIncorrect("menuItem.itemName");
        
        public static readonly Error ItemPriceLessThanZero = GeneralError.ValueIsIncorrect("menuItem.itemPrice");
        
        public static readonly Error ItemOptionsGroupIsNotFound  = GeneralError.ValueIsIncorrect("menuItem.optionsGroup");
        
        public static readonly Error ItemOptionsGroupAlreadyExists = GeneralError.ValueIsIncorrect("menuItem.optionsGroup.name");
    }

    public static class MenuCategory
    {
        public static readonly Error CategoryNameIsEmpty = GeneralError.ValueIsIncorrect("menuCategory.categoryName");
    }

    public static class MenuOptionsGroup
    {
        public static readonly Error OptionAlreadyExists = GeneralError.ValueIsIncorrect("optionsGroup.option");
        
        public static readonly Error OptionsGroupNameIsEmpty = GeneralError.ValueIsIncorrect("optionsGroup.groupName");
    }

    public static class MenuItemOption
    {
        public static readonly Error OptionValueIsEmpty = GeneralError.ValueIsIncorrect("itemOption.value");

        public static readonly Error AdditionalPriceLessThanZero = GeneralError.ValueIsIncorrect("itemOption.additionalPrice");
    }
}