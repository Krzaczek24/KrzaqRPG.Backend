using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KrzaqRPG.Backend.Database.SQLite
{
    public static partial class Injector
    {
        public static IServiceCollection AddAppDatabase(this IServiceCollection services, ILoggerFactory? loggerFactory = null)
            => services
                .AddConnectionStringProvider()
                .AddAppDbContext(loggerFactory)
                .AddTransactionManager()
                .AddAppDbAccesses();

        public static IServiceCollection AddConnectionStringProvider(this IServiceCollection services)
            => services; //.AddTransient<IDbConnectionStringProvider, DbConnectionStringProvider>();

        public static IServiceCollection AddAppDbContext(this IServiceCollection services, ILoggerFactory? loggerFactory = null)
            => services.AddDbContext<Db>((sp, opts) => opts.UseLoggerFactory(loggerFactory));

        public static IServiceCollection AddTransactionManager(this IServiceCollection services)
            => services; //.AddTransient<ITransactionManager, TransactionManager>();

        public static IServiceCollection AddAppDbAccesses(this IServiceCollection services)
        {
            Type injectorType = typeof(Injector);
            var accesses = GetAllNonAbstractFromNamespace(@$"{injectorType.Namespace}.{nameof(Backend.Database.SQLite.Entities)}", DbAccessRegex(), injectorType.Assembly);
            foreach (Type access in accesses)
            {
                Type @interface = access.GetInterface($"I{access.Name}");
                services.AddTransient(@interface, access);
            }
            return services;
        }

        [GeneratedRegex("^Db.+Access$")]
        private static partial Regex DbAccessRegex();

        public static IReadOnlyCollection<Type> GetAllNonAbstractFromNamespace(string @namespace, string? suffixFilter = null, Assembly? assembly = null)
        {
            return (assembly ?? Assembly.GetCallingAssembly())
                .GetTypes()
                .Where(t => t.IsClass
                         && !t.IsAbstract
                         && (t.Namespace?.StartsWith(@namespace) ?? false)
                         && t.Name.EndsWith(suffixFilter ?? string.Empty))
                .ToList()
                .AsReadOnly();
        }

        public static IReadOnlyCollection<Type> GetAllNonAbstractFromNamespace(string @namespace, Regex classNameRegexSelector, Assembly? assembly = null)
        {
            return (assembly ?? Assembly.GetCallingAssembly())
                .GetTypes()
                .Where(t => t.IsClass
                         && !t.IsAbstract
                         && (t.Namespace?.StartsWith(@namespace) ?? false)
                         && classNameRegexSelector.IsMatch(t.Name))
                .ToList()
                .AsReadOnly();
        }
    }
}
