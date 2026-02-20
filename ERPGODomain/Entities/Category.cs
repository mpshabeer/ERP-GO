using System.ComponentModel.DataAnnotations;

namespace ERPGODomain.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Navigation property for SubCategories
  
    public int? DefaultUnitId { get; set; }
    
    [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(DefaultUnitId))]
    public Unit? DefaultUnit { get; set; }
}
