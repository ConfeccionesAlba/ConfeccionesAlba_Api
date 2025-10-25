using ConfeccionesAlba_Api.Models.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ConfeccionesAlba_Api.Data.Interceptors;

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var dbContext = eventData.Context;

        if (dbContext == null)
        {
            return base.SavingChanges(eventData, result);
        }
        
        foreach (var entry in dbContext.ChangeTracker.Entries().Where(IsAddedOrModified))
        {
            if (entry.Entity is not IAuditableEntity auditable)
            {
                continue;
            }
            
            if (entry.State == EntityState.Added)
            {
                auditable.CreatedOn = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                auditable.UpdatedOn = DateTime.UtcNow;
            }
        }

        return base.SavingChanges(eventData, result);
    }

    private static bool IsAddedOrModified(EntityEntry e)
    {
        return e.State is EntityState.Added or EntityState.Modified;
    }
}