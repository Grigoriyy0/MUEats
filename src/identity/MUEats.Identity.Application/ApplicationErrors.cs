using Primitives;

namespace MUEats.Identity.Application;

public static class ApplicationErrors
{
    public static class User
    {
        public static readonly Error UserAlreadyExists = GeneralError.AlreadyExists("email", "user with this email already exists");

        public static readonly Error NameIsEmpty = GeneralError.ValueIsIncorrect("user.name");

        public static readonly Error NotFound = GeneralError.NotFound("user");

        public static readonly Error InvalidDetails = GeneralError.ValueIsIncorrect("user.details", "email or password is incorrect");

        public static readonly Error PasswordDoesNotMatchRequirements = GeneralError.ValueIsIncorrect("user.password", "password does not match the requirements");

        public static readonly Error PasswordsDoNotMatch = GeneralError.ValueIsIncorrect("user.password.confirmation", "passwords do not match");
    }

    public static class Role
    {
        public static readonly Error RoleAlreadyExists = GeneralError.AlreadyExists("rolename", "role already exists");

        public static readonly Error RoleNotFound = GeneralError.NotFound("role");

        public static readonly Error RoleRequirementMissing = GeneralError.ValueIsIncorrect("role", "incorrect required fields value");
    }
}