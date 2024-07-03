using Microsoft.EntityFrameworkCore;

namespace CCSV.Data.EFCore;

public abstract class ApplicationContext : DbContext
{
    protected ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    protected abstract override void OnModelCreating(ModelBuilder modelBuilder);
}
