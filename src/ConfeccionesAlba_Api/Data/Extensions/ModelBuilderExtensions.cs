using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static void SeedIdentityData(this ModelBuilder builder)
    {
        builder.SeedRoles();
        builder.SeedRoleClaims();
    }
}