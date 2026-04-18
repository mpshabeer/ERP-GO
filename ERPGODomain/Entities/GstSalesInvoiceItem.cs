using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ERPGODomain.Entities;

public class GstSalesInvoiceItem
{
    [Key]
    public int Id { get; set; }

    public int GstSalesInvoiceId { get; set; }

    [ForeignKey(nameof(GstSalesInvoiceId))]
    [JsonIgnore]
    public GstSalesInvoice? GstSalesInvoice { get; set; }

    public int ItemId { get; set; }

    [ForeignKey(nameof(ItemId))]
    public Item? Item { get; set; }

    public int UnitId { get; set; }

    [ForeignKey(nameof(UnitId))]
    public Unit? Unit { get; set; }

    [Column(TypeName = "varchar(8)")]
    public string? HSNCode { get; set; }

    public int? ItemUnitId { get; set; }

    [ForeignKey(nameof(ItemUnitId))]
    public ItemUnit? ItemUnit { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal QtyPerBaseUnit { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal Qty { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal QtyInBaseUnit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Rate { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; } // Amount before GST (after discount)

    /// <summary>GST % applicable to this specific item (auto-populated from Item.GstPercent).</summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal GstPercent { get; set; }

    /// <summary>GST Amount = Amount * GstPercent / 100.</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal GstAmount { get; set; }
}
