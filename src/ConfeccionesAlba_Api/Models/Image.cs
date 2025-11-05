using System.ComponentModel.DataAnnotations;

namespace ConfeccionesAlba_Api.Models;

public class Image
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(250)]
    public string Url { get; set; } = string.Empty;
    
    public Item? Item { get; set; }
}