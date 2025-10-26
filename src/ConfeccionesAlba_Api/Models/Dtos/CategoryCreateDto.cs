using System.ComponentModel.DataAnnotations;

namespace ConfeccionesAlba_Api.Models.Dtos;

public class CategoryCreateDto
{
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}