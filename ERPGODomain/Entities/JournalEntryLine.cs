using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class JournalEntryLine
{
    [Key]
    public int Id { get; set; }

    public int JournalEntryId { get; set; }
    
    [ForeignKey("JournalEntryId")]
    public JournalEntry? JournalEntry { get; set; }

    public int AccountId { get; set; }
    
    [ForeignKey("AccountId")]
    public Account? Account { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Debit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Credit { get; set; }

    public string Description { get; set; } = string.Empty;
}
