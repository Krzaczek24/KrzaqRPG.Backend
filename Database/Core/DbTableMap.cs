using Krzaq.Extensions.String;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Core;

internal abstract class DbTableMap<TDbTable> : IEntityTypeConfiguration<TDbTable>
    where TDbTable : DbTable
{
    protected virtual string TableName { get; } = typeof(TDbTable).Name
        .TrimPrefix("Db", ignoreCase: true)
        .TrimSuffix("Table", ignoreCase: true);

    public virtual void Configure(EntityTypeBuilder<TDbTable> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);
    }
}
