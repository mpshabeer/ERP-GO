namespace ERPGODomain.DTOs;

public class PurchaseReportRequest
{
    public DateTime FromDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
    public DateTime ToDate { get; set; } = DateTime.Now.Date;

    public int? SupplierId { get; set; }
    public int? CategoryId { get; set; }
    public int? ItemId { get; set; }
}
