namespace MUEats.Core.Domain.Constants;

public static class RoleConstants
{
    public static class RoleNames
    {
        public const string Admin = "Admin";

        public const string RestaurantOwner = "RestaurantOwner";

        public const string User = "User";
    }

    public static class RoleIds
    {
        public static readonly Guid Admin = Guid.Parse("20fc0a14-837f-457c-baad-ac7b6217098b");

        public static readonly Guid RestaurantOwner = Guid.Parse("b4435c23-7062-4293-a5a6-4c94d0187c32");

        public static readonly Guid User = Guid.Parse("ee0311d6-3f44-472b-b06d-fd352ff54a1c");
    }
}