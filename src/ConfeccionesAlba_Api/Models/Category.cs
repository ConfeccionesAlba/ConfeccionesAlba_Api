using System.ComponentModel.DataAnnotations;
using ConfeccionesAlba_Api.Models.Interfaces;

namespace ConfeccionesAlba_Api.Models;

public class Category : ITrackedEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(250)]
    public string Slug { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}