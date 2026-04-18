using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ERPGODomain.Entities;

public class GstCreditNoteItem
{
    [Key]
    public int Id { get; set; }

    public int GstCreditNoteId { get; set; }

    [ForeignKey(nameof(GstCreditNoteId))]
    [JsonIgnore]
    public GstCreditNote? GstCreditNote { get; set; }

    public int ItemId { get; set; }

    [ForeignKey(nameof(ItemId))]
    public Item? Item { get; set; }

    public int UnitId { get; set; }

    [ForeignKey(nameof(UnitId))]
    public Unit? Unit { get; set; }

    [Column(TypeName = "varchar(8)")]
    public string? HSNCode { get; set; }

    /// <summary>Traceability link back to the original invoice line item.</summary>
    public int? OriginalInvoiceItemId { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal Qty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Rate { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal GstPercent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal GstAmount { get; set; }
}
