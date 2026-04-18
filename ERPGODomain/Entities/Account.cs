using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty; // e.g., Cash, Sales Account, specific Customer/Supplier name

    public string Code { get; set; } = string.Empty; 

    public int AccountHeadId { get; set; }
    
    [ForeignKey("AccountHeadId")]
    public AccountHead? AccountHead { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OpeningBalance { get; set; } 

    // Used to protect system-critical accounts like Cash, Sales, Purchases, or default cash parties
    public bool IsDefault { get; set; } 
    
    public bool IsActive { get; set; } = true;
}
