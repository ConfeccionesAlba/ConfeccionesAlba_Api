using System.ComponentModel.DataAnnotations;

namespace ConfeccionesAlba_Api.Models.Dtos.Items;

public class ItemUpdateDto
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;
    
    public int CategoryId { get; set; }
    
    public decimal PriceReference { get; set; }
    
    public bool IsVisible { get; set; } = true;
}