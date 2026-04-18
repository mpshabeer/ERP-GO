using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

/// <summary>Invoice lifecycle status — once Posted the invoice is locked and immutable.</summary>
public enum InvoiceStatus
{
    Draft  = 0,
    Posted = 1
}

public class GstSalesInvoice
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string InvoiceNo { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Now;

    public int CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer? Customer { get; set; }

    public DateTime? DueDate { get; set; }

    public string PaymentTerms { get; set; } = string.Empty;

    public string SupplyType { get; set; } = "Intra-State";

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    /// <summary>Total GST collected across all line items.</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalGstAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public string Notes { get; set; } = string.Empty;

    // ── Status lifecycle ──────────────────────────────────────────
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime? PostedAt  { get; set; }
    public string?  PostedBy   { get; set; }
    public DateTime CreatedAt  { get; set; } = DateTime.Now;

    public virtual ICollection<GstSalesInvoiceItem>  GstSalesInvoiceItems { get; set; } = new List<GstSalesInvoiceItem>();
    public virtual ICollection<GstCreditNote>        CreditNotes          { get; set; } = new List<GstCreditNote>();
}

