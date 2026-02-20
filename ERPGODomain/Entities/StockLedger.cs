using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class StockLedger
{
    [Key]
    public long Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public int ItemId { get; set; }
    
    [ForeignKey(nameof(ItemId))]
    public Item? Item { get; set; }

    public int? ItemUnitId { get; set; }
    
    [ForeignKey(nameof(ItemUnitId))]
    public ItemUnit? ItemUnit { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal Qty { get; set; } // Positive for In, Negative for Out

    [Column(TypeName = "decimal(18,2)")]
    public decimal Rate { get; set; } // Unit Rate/Cost
    
    public string TransactionType { get; set; } = string.Empty; // "SALES", "PURCHASE", "OPENING", "ADJUSTMENT"

    public string RefId { get; set; } = string.Empty; // Invoice No or Ref No

    public string Notes { get; set; } = string.Empty;
}
