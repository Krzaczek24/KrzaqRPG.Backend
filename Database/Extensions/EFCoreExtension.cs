using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Extensions
{
    internal static class EFCoreExtension
    {
        public static PropertyBuilder<T> HasEnumConversion<T>(this PropertyBuilder<T> builder) where T : struct, Enum
        {
            return builder.HasConversion(x => x.ToString(), x => Enum.Parse<T>(x));
        }
    }
}
