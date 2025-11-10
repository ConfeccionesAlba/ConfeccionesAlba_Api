using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeccionesAlba_Api.Models;

[ComplexType]   
public class Image
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(250)]
    public string Url { get; set; } = string.Empty;
}