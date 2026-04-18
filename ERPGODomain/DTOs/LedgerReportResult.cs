namespace ERPGODomain.DTOs;

public class LedgerReportResult
{
    public string AccountName { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal ClosingBalance { get; set; }
    public List<LedgerReportLine> Lines { get; set; } = new();
}
