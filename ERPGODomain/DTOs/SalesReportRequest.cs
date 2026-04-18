namespace ERPGODomain.DTOs;

public class SalesReportRequest
{
    public DateTime FromDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
    public DateTime ToDate { get; set; } = DateTime.Now.Date;

    public int? CustomerId { get; set; }
    public int? CategoryId { get; set; }
    public int? ItemId { get; set; }
}
