using Primitives;

namespace MUEats.Identity.Core.Domain;

public static class DomainErrors
{
    public static class User
    {
        public static readonly Error UserAttributeKeyIsEmpty = GeneralError.ValueIsIncorrect("user.user_attribute.key");

        public static readonly Error UserAttributeValueIsEmpty = GeneralError.ValueIsIncorrect("user.user_attribute.value");

        public static readonly Error EmailIsEmpty = GeneralError.ValueIsIncorrect("user.email", "email cannot be empty");

        public static readonly Error EmailDoesNotMatch = GeneralError.ValueIsIncorrect("user.email", "email does not match the requirement");

        public static readonly Error NameIsEmpty = GeneralError.ValueIsIncorrect("user.name");

        public static readonly Error PasswordHashIsEmpty = GeneralError.ValueIsIncorrect("user.password_hash");
    }

    public static class Role
    {
        public static readonly Error RoleRequirementValueNameIsEmpty = GeneralError.ValueIsIncorrect("role.role_requirement.value_name", "value name cannot be empty.");

        public static readonly Error RoleNameIsEmpty = GeneralError.ValueIsIncorrect("role.role_name", "role name cannot be empty.");
    }
}