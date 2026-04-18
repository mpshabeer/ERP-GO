namespace ERPGODomain.DTOs;

public class LedgerReportLine
{
    public DateTime Date { get; set; }
    public string VoucherNo { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
}
