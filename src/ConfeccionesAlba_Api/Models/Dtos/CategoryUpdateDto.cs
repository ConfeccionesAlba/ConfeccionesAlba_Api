using System.ComponentModel.DataAnnotations;

namespace ConfeccionesAlba_Api.Models.Dtos;

public class CategoryUpdateDto
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}