using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class OpeningStockHistory
{
    [Key]
    public int Id { get; set; }

    public int ItemId { get; set; }
    
    [Column(TypeName = "decimal(18,3)")]
    public decimal Qty { get; set; } // Base Unit Qty

    [Column(TypeName = "decimal(18,2)")]
    public decimal Rate { get; set; }

    public int? ItemUnitId { get; set; }
    
    public DateTime Date { get; set; } = DateTime.Now;

    public string Action { get; set; } = string.Empty; // "Created", "Updated", "Replaced"

    public string Notes { get; set; } = string.Empty;
    
    public string UserId { get; set; } = string.Empty; // Optional if we track user

    [ForeignKey(nameof(ItemId))]
    public virtual Item Item { get; set; }
}
