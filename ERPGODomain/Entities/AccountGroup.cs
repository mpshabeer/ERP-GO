using System.ComponentModel.DataAnnotations;

namespace ERPGODomain.Entities;

public class AccountGroup
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty; // e.g., Assets, Liabilities, Equity, Revenue, Expenses
    
    public bool IsActive { get; set; } = true;
}
