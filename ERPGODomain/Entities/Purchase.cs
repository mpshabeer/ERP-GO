using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class Purchase
{
    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public int SupplierId { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public Supplier? Supplier { get; set; }

    [Required]
    public string InvoiceNo { get; set; } = string.Empty; // Supplier's Invoice No

    public string? PurchaseNo { get; set; } // Internal Purchase No (e.g., PUR001)

    public DateTime? DueDate { get; set; }

    public string? PaymentTerms { get; set; }

    public string Notes { get; set; } = string.Empty;

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

    public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
}
