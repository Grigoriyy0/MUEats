namespace MUEats.Application.Queries;

public class GetUsersQuery
{
    public string RoleName { get; set; } = "User";

    public int PageSize { get; set; } = 20;

    public int Page { get; set; } = 1;
}