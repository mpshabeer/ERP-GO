namespace ERPGODomain.DTOs;

public class PaymentRequest
{
    public int Id { get; set; }
    public string PaymentNo { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;
    
    public int CashBankAccountId { get; set; }
    public int PartyAccountId { get; set; }
}
