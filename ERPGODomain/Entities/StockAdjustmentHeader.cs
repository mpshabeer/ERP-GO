using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class StockAdjustmentHeader
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string AdjustmentNo { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Now;

    [MaxLength(100)]
    public string AdjustedBy { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,3)")]
    public decimal TotalQty { get; set; } // Sum of AdjustedQty (absolute or net?) - Let's simplify as just sum for list view if needed

    public virtual ICollection<StockAdjustmentDetail> Details { get; set; } = new List<StockAdjustmentDetail>();
}
