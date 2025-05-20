using CCSV.Domain.Entities;
using CCSV.Domain.Exceptions;
using CCSV.Domain.Repositories.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CCSV.Data.EFCore;

public abstract class DerivedRepository<Tentity, Tbase> : Repository<Tentity> where Tentity : Tbase where Tbase : Entity
{
    private readonly ApplicationContext Context;

    protected DerivedRepository(ApplicationContext context) : base(context)
    {
        Context = context;
    }

    public async override Task Create(Tentity entity)
    {
        if (entity is null)
        {
            throw new ArgumentEntityException($"User try to create a null entity ({typeof(Tentity).Name}).");
        }

        if (entity.Id.Equals(Guid.Empty))
        {
            throw new ArgumentEntityException($"Create {typeof(Tentity).Name} (Id: {entity.Id}) exception.");
        }

        if (await Context.Set<Tbase>().ContainsAsync(entity))
        {
            throw new DuplicatedValueException($"The {typeof(Tbase).Name} (Id: {entity.Id}) is already in data base.");
        }

        try
        {
            Context.Add<Tentity>(entity);
        }
        catch (Exception ex)
        {
            throw new InternalRepositoryException($"Create {typeof(Tentity).Name} (Id: {entity.Id}) exception.", ex);
        }
    }
}
