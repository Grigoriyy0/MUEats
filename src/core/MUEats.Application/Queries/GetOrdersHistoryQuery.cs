namespace MUEats.Application.Queries;

public class GetOrdersHistoryQuery
{
    public DateTime FromDate { get; set; }
    
    public DateTime ToDate { get; set; }

    public int PageSize { get; set; } = 10;

    public int Page { get; set; } = 1;
}