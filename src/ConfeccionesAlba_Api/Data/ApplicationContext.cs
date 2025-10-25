using ConfeccionesAlba_Api.Model;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<Item> Items { get; set; }
    public DbSet<Category> Categories { get; set; }
}