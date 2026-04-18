using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class GstCreditNote
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string CreditNoteNo { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Now;

    // ── Reference to original invoice ─────────────────────────────
    public int OriginalInvoiceId { get; set; }

    [ForeignKey(nameof(OriginalInvoiceId))]
    public GstSalesInvoice? OriginalInvoice { get; set; }

    public int CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer? Customer { get; set; }

    /// <summary>Reason: GoodsReturn | PriceCorrection | ItemMistake | Other</summary>
    public string Reason { get; set; } = "GoodsReturn";

    public string Notes { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalGstAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    // ── Audit trail ───────────────────────────────────────────────
    public DateTime CreatedAt  { get; set; } = DateTime.Now;
    public string?  CreatedBy  { get; set; }

    public virtual ICollection<GstCreditNoteItem> Items { get; set; } = new List<GstCreditNoteItem>();
}
