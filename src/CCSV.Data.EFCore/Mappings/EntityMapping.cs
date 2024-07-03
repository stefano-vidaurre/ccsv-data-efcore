using CCSV.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CCSV.Data.EFCore.Mappings;

public abstract class EntityMapping<Tentity> : IEntityTypeConfiguration<Tentity> where Tentity : Entity
{
    protected abstract string TableName { get; }

    public void Configure(EntityTypeBuilder<Tentity> builder)
    {
        builder.ToTable(TableName);
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).ValueGeneratedNever();
        builder.Property(entity => entity.EntityConcurrencyToken).IsConcurrencyToken();

        ConfigureMapping(builder);
    }

    protected abstract void ConfigureMapping(EntityTypeBuilder<Tentity> builder);
}
