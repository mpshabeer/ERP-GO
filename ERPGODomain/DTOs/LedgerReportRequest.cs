namespace ERPGODomain.DTOs;

public class LedgerReportRequest
{
    public int AccountId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
