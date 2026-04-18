namespace ERPGODomain.DTOs;

public class TrialBalanceLine
{
    public string AccountGroupName { get; set; } = string.Empty;
    public string AccountHeadName { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal DebitBalance { get; set; }
    public decimal CreditBalance { get; set; }
}
