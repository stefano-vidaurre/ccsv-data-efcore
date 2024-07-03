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

    public async Task<int> Count(Func<IQueryable<Tentity>, IQueryable<Tentity>>? query = null)
    {
        if (query is not null)
        {
            return await query(Context.Set<Tentity>()).CountAsync();
        }
        else
        {
            return await Context.Set<Tentity>().CountAsync();
        }
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

    public abstract Task<Tentity?> GetByIdOrDefault(Guid id);

    public async Task<IEnumerable<Tentity>> GetAll(Func<IQueryable<Tentity>, IQueryable<Tentity>>? query = null)
    {
        IQueryable<Tentity> entities;

        if (query is not null)
        {
            entities = query(Context.Set<Tentity>());
        }
        else
        {
            entities = Context.Set<Tentity>();
        }

        return await entities.ToArrayAsync();
    }

    public async Task<Tentity> GetById(Guid id)
    {
        Tentity? entity = await GetByIdOrDefault(id);

        if (entity is null)
        {
            throw new ValueNotFoundException($"The {typeof(Tentity).Name} (Id: {id}) not found.");
        }

        return entity;
    }

    public Task Delete(Tentity entity)
    {
        if (entity is null)
        {
            throw new ArgumentEntityException($"User try to delete a null entity ({typeof(Tentity).Name}).");
        }

        try
        {
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

    public async Task<bool> Any(Func<IQueryable<Tentity>, IQueryable<Tentity>>? query = null)
    {
        return await Count(query) > 0;
    }
}
