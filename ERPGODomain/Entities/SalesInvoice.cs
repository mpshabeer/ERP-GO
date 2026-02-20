using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class SalesInvoice
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

    public string PaymentTerms { get; set; } = string.Empty; // e.g., "Net 30", "Cash", "Credit"

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxPercent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public string Notes { get; set; } = string.Empty;

    public virtual ICollection<SalesInvoiceItem> SalesInvoiceItems { get; set; } = new List<SalesInvoiceItem>();
}
