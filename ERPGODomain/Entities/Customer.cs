using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPGODomain.Entities;

public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string TaxNumber { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal CreditLimit { get; set; }

    public bool IsActive { get; set; } = true;

    // Link to Chart of Accounts (Sundry Debtors)
    public int? AccountId { get; set; }
    
    [ForeignKey("AccountId")]
    public Account? Account { get; set; }
}
