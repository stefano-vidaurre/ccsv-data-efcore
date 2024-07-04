using CCSV.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CCSV.Data.EFCore;

public class Transaction: ITransaction
{
    private readonly ApplicationContext Context;

    public Transaction(ApplicationContext context)
    {
        Context = context;
    }

    public async Task<TResult> Run<TResult>(Func<Task<TResult>> func)
    {
        using IDbContextTransaction transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            TResult result = await func();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task Run(Func<Task> action)
    {
        using IDbContextTransaction transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public TResult Run<TResult>(Func<TResult> func)
    {
        using IDbContextTransaction transaction = Context.Database.BeginTransaction();
        try
        {
            TResult result = func();
            transaction.Commit();
            return result;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void Run(Action action)
    {
        using IDbContextTransaction transaction = Context.Database.BeginTransaction();
        try
        {
            action();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
