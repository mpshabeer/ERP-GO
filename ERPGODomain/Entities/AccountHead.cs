using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class AccountHead
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty; // e.g., Current Assets, Sundry Debtors

    public int AccountGroupId { get; set; }
    
    [ForeignKey("AccountGroupId")]
    public AccountGroup? AccountGroup { get; set; }
    
    public bool IsActive { get; set; } = true;
}
