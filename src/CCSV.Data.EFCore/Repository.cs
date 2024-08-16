using CCSV.Domain.Entities;
using CCSV.Domain.Exceptions;
using CCSV.Domain.Repositories;
using CCSV.Domain.Repositories.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CCSV.Data.EFCore;

public abstract class Repository<Tentity> : IRepository<Tentity> where Tentity : Entity
{
    private readonly ApplicationContext Context;

    protected Repository(ApplicationContext context)
    {
        Context = context;
    }

    public async Task<int> Count(bool deletionsIncluded = false)
    {
        if(deletionsIncluded) {
            return await Context.Set<Tentity>().CountAsync();
        }

        return await Context.Set<Tentity>().Where(entity => entity.EntityDeletionDate == null).CountAsync();
    }

    public async Task<int> Count(Func<IQueryable<Tentity>, IQueryable<Tentity>> query, bool deletionsIncluded = false)
    {
        if(deletionsIncluded) {
            return await query(Context.Set<Tentity>()).CountAsync();
        }

        return await query(Context.Set<Tentity>().Where(entity => entity.EntityDeletionDate == null)).CountAsync();
    }

    public async Task<bool> Any(bool deletionsIncluded = false)
    {
        return await Count(deletionsIncluded) > 0;
    }

    public async Task<bool> Any(Func<IQueryable<Tentity>, IQueryable<Tentity>> query, bool deletionsIncluded = false)
    {
        return await Count(query, deletionsIncluded) > 0;
    }

    public async Task<Tentity?> FindOrDefault(Guid id)
    {
        return await Context.FindAsync<Tentity>(id);
    }

    public async Task<Tentity> Find(Guid id)
    {
        Tentity? entity = await FindOrDefault(id);

        if (entity is null)
        {
            throw new ValueNotFoundException($"The {typeof(Tentity).Name} (Id: {id}) not found.");
        }

        return entity;
    }

    public async Task<IEnumerable<Tentity>> GetAll(bool deletionsIncluded = false)
    {
        if(deletionsIncluded) {
            return await Context.Set<Tentity>().ToArrayAsync();
        }

        return await Context.Set<Tentity>().Where(entity => entity.EntityDeletionDate == null).ToArrayAsync();
    }

    public async Task<IEnumerable<Tentity>> GetAll(Func<IQueryable<Tentity>, IQueryable<Tentity>> query, bool deletionsIncluded = false)
    {
        if(deletionsIncluded) {
            return await query(Context.Set<Tentity>()).ToArrayAsync();
        }

        return await query(Context.Set<Tentity>().Where(entity => entity.EntityDeletionDate == null)).ToArrayAsync();
    }

    public abstract Task<Tentity?> GetByIdOrDefault(Guid id);

    public async Task<Tentity> GetById(Guid id)
    {
        Tentity? entity = await GetByIdOrDefault(id);

        if (entity is null)
        {
            throw new ValueNotFoundException($"The {typeof(Tentity).Name} (Id: {id}) not found.");
        }

        return entity;
    }

    public async Task Create(Tentity entity)
    {
        if (entity is null)
        {
            throw new ArgumentEntityException($"User try to create a null entity ({typeof(Tentity).Name}).");
        }

        try
        {
            if (entity.Id.Equals(Guid.Empty))
            {
                throw new ArgumentEntityException($"Create {typeof(Tentity).Name}(Id: {entity.Id}) exception.");
            }

            if (await Context.Set<Tentity>().ContainsAsync(entity))
            {
                throw new DuplicatedValueException($"The {typeof(Tentity).Name}(Id: {entity.Id}) is already in data base.");
            }

            Context.Add<Tentity>(entity);
        }
        catch (Exception ex)
        {
            throw new InternalRepositoryException($"Create {typeof(Tentity).Name}(Id: {entity.Id}) exception.", ex);
        }
    }

    public Task Update(Tentity entity)
    {
        if (entity is null)
        {
            throw new ArgumentEntityException($"User try to update a null entity ({typeof(Tentity).Name}).");
        }

        try
        {
            if (entity.Id.Equals(Guid.Empty))
            {
                throw new ArgumentEntityException($"Update {typeof(Tentity).Name}(Id: {entity.Id}) exception.");
            }

            Context.Update<Tentity>(entity);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new InternalRepositoryException($"Update {typeof(Tentity).Name}(Id: {entity.Id}) exception.", ex);
        }
    }

    public Task Delete(Tentity entity)
    {
        if (entity is null)
        {
            throw new ArgumentEntityException($"User try to delete a null entity ({typeof(Tentity).Name}).");
        }

        try
        {
            if (entity.Id.Equals(Guid.Empty))
            {
                throw new ArgumentEntityException($"Delete {typeof(Tentity).Name}(Id: {entity.Id}) exception.");
            }

            Context.Remove<Tentity>(entity);

            return Task.CompletedTask;
        }
        catch (InvalidOperationException ex)
        {
            throw new RepositoryOperationException($"{typeof(Tentity).Name} can't be deleted. Check cascade restrictions.", ex);
        }
        catch (Exception ex)
        {
            throw new InternalRepositoryException($"Delete {typeof(Tentity).Name}(Id: {entity.Id}) exception.", ex);
        }
    }
}
