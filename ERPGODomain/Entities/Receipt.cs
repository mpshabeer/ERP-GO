using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class Receipt
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ReceiptNo { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public string Narration { get; set; } = string.Empty;

    // The account receiving the money (e.g., Cash or Bank) - This account gets DEBITED
    public int CashBankAccountId { get; set; }
    
    [ForeignKey("CashBankAccountId")]
    public Account? CashBankAccount { get; set; }

    // The account giving the money (e.g., Customer or Income source) - This account gets CREDITED
    public int PartyAccountId { get; set; }

    [ForeignKey("PartyAccountId")]
    public Account? PartyAccount { get; set; }
}
