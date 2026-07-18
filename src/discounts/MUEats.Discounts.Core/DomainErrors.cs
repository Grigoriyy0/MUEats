using Primitives;

namespace MUEats.Discounts.Core;

public static class DomainErrors
{
    public static class Discount
    {
        public static readonly Error InvalidFixedValue = 
            GeneralError.ValueIsIncorrect("discount.value", "Fixed discount value must be strictly greater than zero.");
            
        public static readonly Error InvalidPercentageValue = 
            GeneralError.ValueIsIncorrect("discount.value", "Percentage discount must be between 0 and 100.");
            
        public static readonly Error TargetRolesRequired = 
            GeneralError.ValueIsIncorrect("discount.targetRoles", "At least one target role must be specified.");
            
        public static readonly Error InvalidExpirationDate = 
            GeneralError.ValueIsIncorrect("discount.activeBefore", "Expiration date cannot be earlier than the activation date.");
            
        public static readonly Error InvalidMaxUsageCount = 
            GeneralError.ValueIsIncorrect("discount.maxUsageCount", "Max usage count must be greater than zero.");
            
        public static readonly Error NegativeMinOrderValue = 
            GeneralError.ValueIsIncorrect("discount.minOrderValue", "Minimum order value cannot be negative.");

        public static readonly Error TargetRoleName =
            GeneralError.ValueIsIncorrect("discount.targetRole.roleName", "Role name cannot be empty.");
    }
}