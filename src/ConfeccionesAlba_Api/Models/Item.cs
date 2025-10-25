using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConfeccionesAlba_Api.Models.Interfaces;

namespace ConfeccionesAlba_Api.Models;

public class Item : ITrackedEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;
    
    public int CategoryId { get; set; }
    
    [ForeignKey("FK_Item_Category_CategoryId")]
    public Category? Category { get; set; }
    
    public decimal PriceReference { get; set; }
    
    public string ImageUrl { get; set; } = string.Empty;
    
    public bool IsVisible { get; set; } = true;

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}