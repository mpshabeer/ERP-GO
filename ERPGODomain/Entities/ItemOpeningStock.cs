using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class ItemOpeningStock
{
    [Key]
    public int Id { get; set; }

    public int ItemId { get; set; }
    [ForeignKey(nameof(ItemId))]
    public Item? Item { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal Qty { get; set; } // Base Unit Qty

    [Column(TypeName = "decimal(18,2)")]
    public decimal Rate { get; set; }

    public int? ItemUnitId { get; set; } // Specific packing used for entry
    [ForeignKey(nameof(ItemUnitId))]
    public ItemUnit? ItemUnit { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalValue { get; set; }

    public string Notes { get; set; } = string.Empty;
}
