using System.ComponentModel.DataAnnotations;

namespace ConfeccionesAlba_Api.Models.Dtos.Items;

public class ItemRequestDto
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;
    
    public int CategoryId { get; set; }
    
    public decimal PriceReference { get; set; }
    
    public string ImageUrl { get; set; } = string.Empty;
    
    public bool IsVisible { get; set; } = true;
}