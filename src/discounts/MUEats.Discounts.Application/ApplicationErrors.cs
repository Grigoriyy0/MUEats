using Primitives;

namespace MUEats.Discounts.Application;

public static class ApplicationErrors
{
    public static class Discount
    {
        public static readonly Error AlreadyExists = GeneralError.AlreadyExists("discount", "discount with such parameters already exists");
    }
}