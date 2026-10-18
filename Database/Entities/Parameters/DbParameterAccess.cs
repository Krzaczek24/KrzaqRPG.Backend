using KrzaqTools.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static KrzaqRPG.Backend.Common.Enums.Parameter;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Parameters;

public interface IDbParameterAccess
{
    Task<DbParameter?> GetParameterAsync(ParamKey key, CancellationToken cancellationToken = default);
    Task<string?> GetStringParameterAsync(ParamKey key, CancellationToken cancellationToken = default);
    Task<DateTime?> GetDateTimeParameterAsync(ParamKey key, CancellationToken cancellationToken = default);
    Task<TimeSpan?> GetTimeSpanParameterAsync(ParamKey key, CancellationToken cancellationToken = default);
    Task<double?> GetNumericParameterAsync(ParamKey key, CancellationToken cancellationToken = default);
    Task<bool?> GetBooleanParameterAsync(ParamKey key, CancellationToken cancellationToken = default);

    Task<bool> SetParameterValueAsync(ParamKey key, string? value, CancellationToken cancellationToken = default);
    Task<bool> SetParameterValueAsync(ParamKey key, DateTime? value, CancellationToken cancellationToken = default);
    Task<bool> SetParameterValueAsync(ParamKey key, TimeSpan? value, CancellationToken cancellationToken = default);
    Task<bool> SetParameterValueAsync(ParamKey key, double? value, CancellationToken cancellationToken = default);
    Task<bool> SetParameterValueAsync(ParamKey key, bool? value, CancellationToken cancellationToken = default);
}

internal class DbParameterAccess(Db db, IMemoryCache cache) : IDbParameterAccess
{
    private IQueryable<DbParameter> BaseQuery(ParamKey key)
    {
        string name = cache.GetOrCreate(key, _ => key.GetDescription());
        return db.Parameters.Where(p => p.Name == name);
    }

    public Task<DbParameter?> GetParameterAsync(ParamKey key, CancellationToken cancellationToken = default)
    {
        return BaseQuery(key).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<string?> GetStringParameterAsync(ParamKey key, CancellationToken cancellationToken = default)
    {
        return BaseQuery(key).Select(p => p.String).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<DateTime?> GetDateTimeParameterAsync(ParamKey key, CancellationToken cancellationToken = default)
    {
        return BaseQuery(key).Select(p => p.DateTime).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TimeSpan?> GetTimeSpanParameterAsync(ParamKey key, CancellationToken cancellationToken = default)
    {
        return BaseQuery(key).Select(p => p.TimeSpan).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<double?> GetNumericParameterAsync(ParamKey key, CancellationToken cancellationToken = default)
    {
        return BaseQuery(key).Select(p => p.Numeric).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool?> GetBooleanParameterAsync(ParamKey key, CancellationToken cancellationToken = default)
    {
        return BaseQuery(key).Select(p => p.Boolean).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> SetParameterValueAsync(ParamKey key, string? value, CancellationToken cancellationToken = default)
    {
        return await BaseQuery(key).ExecuteUpdateAsync(update => update.SetProperty(p => p.String, value), cancellationToken) > 0;
    }
    public async Task<bool> SetParameterValueAsync(ParamKey key, DateTime? value, CancellationToken cancellationToken = default)
    {
        return await BaseQuery(key).ExecuteUpdateAsync(update => update.SetProperty(p => p.DateTime, value), cancellationToken) > 0;
    }
    public async Task<bool> SetParameterValueAsync(ParamKey key, TimeSpan? value, CancellationToken cancellationToken = default)
    {
        return await BaseQuery(key).ExecuteUpdateAsync(update => update.SetProperty(p => p.TimeSpan, value), cancellationToken) > 0;
    }
    public async Task<bool> SetParameterValueAsync(ParamKey key, double? value, CancellationToken cancellationToken = default)
    {
        return await BaseQuery(key).ExecuteUpdateAsync(update => update.SetProperty(p => p.Numeric, value), cancellationToken) > 0;
    }
    public async Task<bool> SetParameterValueAsync(ParamKey key, bool? value, CancellationToken cancellationToken = default)
    {
        return await BaseQuery(key).ExecuteUpdateAsync(update => update.SetProperty(p => p.Boolean, value), cancellationToken) > 0;
    }
}
