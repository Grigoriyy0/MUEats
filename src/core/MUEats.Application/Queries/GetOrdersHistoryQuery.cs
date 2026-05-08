namespace MUEats.Application.Queries;

public class GetOrdersHistoryQuery
{
    public DateTime FromDate { get; set; }
    
    public DateTime ToDate { get; set; }
    
    public int PageSize { get; set; }
    
    public int Page { get; set; }
}