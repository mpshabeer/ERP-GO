
using System.ComponentModel.DataAnnotations;

namespace ERPGODomain.Entities;

public class Unit
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public int DecimalPlaces { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
}
