using ConfeccionesAlba_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConfeccionesAlba_Api.Data.Configurations;

public class ItemEntityTypeConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasOne(i => i.Image)
            .WithOne(i => i.Item)
            .HasForeignKey<Item>(i => i.ImageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull); // Opcional: qué pasa al eliminar

        // Hacer ImageId único para garantizar relación 1:1
        builder.HasIndex(i => i.ImageId)
            .IsUnique();
    }
}