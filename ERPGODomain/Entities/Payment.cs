using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string PaymentNo { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public string Narration { get; set; } = string.Empty;

    // The account paying the money (e.g., Cash or Bank) - This account gets CREDITED
    public int CashBankAccountId { get; set; }
    
    [ForeignKey("CashBankAccountId")]
    public Account? CashBankAccount { get; set; }

    // The account receiving the money (e.g., Supplier) - This account gets DEBITED
    public int PartyAccountId { get; set; }

    [ForeignKey("PartyAccountId")]
    public Account? PartyAccount { get; set; }
}
