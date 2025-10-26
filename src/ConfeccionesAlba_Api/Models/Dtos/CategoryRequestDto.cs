using System.ComponentModel.DataAnnotations;

namespace ConfeccionesAlba_Api.Models.Dtos;

public class CategoryRequestDto
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(250)]
    public string Slug { get; set; } = string.Empty;
}