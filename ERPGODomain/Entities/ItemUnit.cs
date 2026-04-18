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

    /// <summary>
    /// How many base units make one of this packing unit. E.g. 12 if 1 Box = 12 PCS.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal QtyPerBaseUnit { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Rate { get; set; } // Selling Price for this unit

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchaseRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal WholesaleRate { get; set; }
}
