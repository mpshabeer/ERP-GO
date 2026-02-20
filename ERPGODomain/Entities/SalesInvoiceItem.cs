using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class SalesInvoiceItem
{
    [Key]
    public int Id { get; set; }

    public int SalesInvoiceId { get; set; }

    [ForeignKey(nameof(SalesInvoiceId))]
    public SalesInvoice? SalesInvoice { get; set; }

    public int ItemId { get; set; }
    
    [ForeignKey(nameof(ItemId))]
    public Item? Item { get; set; }

    public int UnitId { get; set; }
    
    [ForeignKey(nameof(UnitId))]
    public Unit? Unit { get; set; }

    public int? ItemUnitId { get; set; }

    [ForeignKey(nameof(ItemUnitId))]
    public ItemUnit? ItemUnit { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal Qty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Rate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }
    
    // For Stock Deduction (Qty * Conversion Factor)
    [Column(TypeName = "decimal(18,3)")]
    public decimal QtyInBaseUnit { get; set; } 
}
