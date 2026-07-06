

using Primitives;

namespace MUEats.Application;

public static class ApplicationErrors
{
    public static class User
    {
        public static readonly Error UserAlreadyExists = GeneralError.AlreadyExists("email", "user with this email already exists");

        public static readonly Error NameIsEmpty = GeneralError.ValueIsIncorrect("user.name");

        public static readonly Error NotFound = GeneralError.NotFound("user");

        public static readonly Error InvalidDetails = GeneralError.ValueIsIncorrect("user.details", "login or password is incorrect");
    }

    public static class Role
    {
        public static readonly Error RoleAlreadyExists = GeneralError.AlreadyExists("rolename", "role already exists");

        public static readonly Error RoleNotFound = GeneralError.NotFound("role");
    }
}