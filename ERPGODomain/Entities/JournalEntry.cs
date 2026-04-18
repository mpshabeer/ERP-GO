using System.ComponentModel.DataAnnotations;

namespace ERPGODomain.Entities;

public class JournalEntry
{
    [Key]
    public int Id { get; set; }

    public DateTime VoucherDate { get; set; } = DateTime.Now;

    [Required]
    public string VoucherNo { get; set; } = string.Empty; // e.g., JV-0001, SI-0001

    [Required]
    public string VoucherType { get; set; } = string.Empty; // e.g., "Sales", "Purchase", "JournalVoucher"

    public int? ReferenceId { get; set; } // Links to SalesInvoice.Id or Purchase.Id

    public string Narration { get; set; } = string.Empty;

    public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}
