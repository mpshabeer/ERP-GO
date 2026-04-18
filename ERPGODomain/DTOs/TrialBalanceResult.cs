namespace ERPGODomain.DTOs;

public class TrialBalanceResult
{
    public DateTime AsOfDate { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public List<TrialBalanceLine> Lines { get; set; } = new();
}
