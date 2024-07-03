using CCSV.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CCSV.Data.EFCore.Mappings;

public abstract class DerivedEntityMapping<Tentity, Tbase> : IEntityTypeConfiguration<Tentity> where Tentity : Tbase where Tbase : Entity
{
    public void Configure(EntityTypeBuilder<Tentity> builder)
    {
        builder.HasBaseType<Tbase>();

        ConfigureMapping(builder);
    }

    protected abstract void ConfigureMapping(EntityTypeBuilder<Tentity> builder);
}
