using ConfeccionesAlba_Api.Data.Interceptors;
using ConfeccionesAlba_Api.Model;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Data;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.AddInterceptors(new AuditableEntityInterceptor());

    public DbSet<Item> Items { get; set; }
    public DbSet<Category> Categories { get; set; }
}