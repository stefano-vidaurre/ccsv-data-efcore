using CCSV.Domain.Entities;
using CCSV.Domain.Repositories;
using CCSV.Domain.Repositories.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CCSV.Data.EFCore;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext Context;

    public UnitOfWork(ApplicationContext context)
    {
        Context = context;
    }

    public void UpdateEditDateTimes()
    {
        IEnumerable<EntityEntry> entries = Context
                .ChangeTracker
                .Entries()
                .Where(e => e.Entity is Entity && e.State == EntityState.Modified);

        foreach (EntityEntry entity in entries)
        {
            ((Entity)entity.Entity).SetAsEdited();
        }
    }

    public async Task SaveAsync()
    {
        try
        {
            await Context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new EntityUpdateConcurrencyException("Writing conflict between multiple simultaneous updates.", ex);
        }
        catch (Exception ex)
        {
            throw new InternalRepositoryException("Data base commit exception.", ex);
        }
    }

    public void Save()
    {
        try
        {
            Context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new EntityUpdateConcurrencyException("Writing conflict between multiple simultaneous updates.", ex);
        }
        catch (Exception ex)
        {
            throw new InternalRepositoryException("Data base commit exception.", ex);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Context?.Dispose();
        }
    }
}