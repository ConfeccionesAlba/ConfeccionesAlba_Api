using ConfeccionesAlba_Api.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ConfeccionesAlba_Api.Data.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var dbContext = eventData.Context;

        if (dbContext == null)
        {
            return base.SavingChanges(eventData, result);
        }
        
        UpdateAuditableEntities(dbContext);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, 
        InterceptionResult<int> result, CancellationToken cancellationToken = new())
    {
        var dbContext = eventData.Context;

        if (dbContext == null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        
        UpdateAuditableEntities(dbContext);
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateAuditableEntities(DbContext dbContext)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries().Where(IsAddedOrModified))
        {
            var currentTime = DateTime.UtcNow;
            
            if (entry.Entity is not IAuditableEntity auditable)
            {
                continue;
            }
            
            if (entry.State == EntityState.Added)
            {
                auditable.CreatedOn = currentTime;
                auditable.UpdatedOn = currentTime;
            }
            else if (entry.State == EntityState.Modified)
            {
                auditable.UpdatedOn = currentTime;
            }
        }
    }

    private static bool IsAddedOrModified(EntityEntry e)
    {
        return e.State is EntityState.Added or EntityState.Modified;
    }
}