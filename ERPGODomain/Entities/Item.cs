
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class Item
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string ItemCode { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;

    public int UnitId { get; set; }
    
    [ForeignKey(nameof(UnitId))]
    public Unit? BaseUnit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Rate { get; set; } // Selling Rate (Base)

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchaseRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal WholesaleRate { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal CurrentStock { get; set; }

    public bool IsMultiUnit { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual ICollection<ItemUnit> ItemUnits { get; set; } = new List<ItemUnit>();
}
