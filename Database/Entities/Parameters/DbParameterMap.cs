using KrzaqRPG.Backend.Database.SQLite.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Parameters
{
    internal class DbParameterMap : DbTableMap<DbParameter>
    {
        public override void Configure(EntityTypeBuilder<DbParameter> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(x => x.String)
                .HasColumnName("StringValue");

            builder.Property(x => x.DateTime)
                .HasColumnName("DateTimeValue");

            builder.Property(x => x.TimeSpan)
                .HasColumnName("TimeSpanValue");

            builder.Property(x => x.Numeric)
                .HasColumnName("NumericValue");

            builder.Property(x => x.Boolean)
                .HasColumnName("BooleanValue");
        }
    }
}
