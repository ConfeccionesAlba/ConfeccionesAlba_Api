using Microsoft.AspNetCore.Identity;

namespace ConfeccionesAlba_Api.Model;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}