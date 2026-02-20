using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class StockAdjustmentDetail
{
    [Key]
    public int Id { get; set; }

    public int HeaderId { get; set; }

    [ForeignKey(nameof(HeaderId))]
    public virtual StockAdjustmentHeader Header { get; set; }

    public int ItemId { get; set; }

    [ForeignKey(nameof(ItemId))]
    public virtual Item Item { get; set; }

    public int? ItemUnitId { get; set; }
    
    [ForeignKey(nameof(ItemUnitId))]
    public virtual ItemUnit? ItemUnit { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal SystemQty { get; set; } // Snapshot of Current Stock

    [Column(TypeName = "decimal(18,3)")]
    public decimal NewQty { get; set; } // Physical Stock

    [Column(TypeName = "decimal(18,3)")]
    public decimal AdjustedQty { get; set; } // Difference: NewQty - SystemQty

    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;
}
