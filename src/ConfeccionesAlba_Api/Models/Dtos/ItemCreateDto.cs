using System.ComponentModel.DataAnnotations;

namespace ConfeccionesAlba_Api.Models.Dtos;

public class ItemCreateDto
{
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;
    
    public int CategoryId { get; set; }
    
    public decimal PriceReference { get; set; }
    
    public bool IsVisible { get; set; } = true;
}