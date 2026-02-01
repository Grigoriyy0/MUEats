namespace MUEats.Core.Domain.User.ValueObjects;

public class Name
{
    private string FirstName { get; set; } = null!;

    private string LastName { get; set; } = null!;

    public string Value { get; set; } = null!;
}