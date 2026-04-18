namespace ERPGODomain.DTOs;

public class ExpenseRequest
{
    public int Id { get; set; }
    public string ExpenseNo { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;
    
    public int CashBankAccountId { get; set; }
    public int ExpenseAccountId { get; set; }
}
