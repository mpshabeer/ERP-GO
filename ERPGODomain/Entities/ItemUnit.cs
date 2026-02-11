using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class ItemUnit
{
    [Key]
    public int Id { get; set; }

    public int ItemId { get; set; }
    
    [ForeignKey(nameof(ItemId))]
    public Item? Item { get; set; }

    public int UnitId { get; set; }
    
    [ForeignKey(nameof(UnitId))]
    public Unit? Unit { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;
    
    public string ItemUnitCode { get; set; } = string.Empty; // Code specific to this packing

    [Column(TypeName = "decimal(18,2)")]
    public decimal QtyPerBaseUnit { get; set; } // Conversion Factor

    [Column(TypeName = "decimal(18,2)")]
    public decimal Rate { get; set; } // Selling Price for this unit

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchaseRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal WholesaleRate { get; set; }
}
