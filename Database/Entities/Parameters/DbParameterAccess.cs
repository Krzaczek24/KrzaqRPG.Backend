using KrzaqTools.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static KrzaqRPG.Backend.Common.Enums.Parameter;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Parameters;

public interface IDbParameterAccess
{
    Task<DbParameter?> GetParameterAsync(ParamKey key, CancellationToken ct = default);
    Task<string?> GetStringParameterAsync(ParamKey key, CancellationToken ct = default);
    Task<DateTime?> GetDateTimeParameterAsync(ParamKey key, CancellationToken ct = default);
    Task<TimeSpan?> GetTimeSpanParameterAsync(ParamKey key, CancellationToken ct = default);
    Task<double?> GetNumericParameterAsync(ParamKey key, CancellationToken ct = default);
    Task<bool?> GetBooleanParameterAsync(ParamKey key, CancellationToken ct = default);

    Task<bool> SetParameterValueAsync(ParamKey key, string? value, CancellationToken ct = default);
    Task<bool> SetParameterValueAsync(ParamKey key, DateTime? value, CancellationToken ct = default);
    Task<bool> SetParameterValueAsync(ParamKey key, TimeSpan? value, CancellationToken ct = default);
    Task<bool> SetParameterValueAsync(ParamKey key, double? value, CancellationToken ct = default);
    Task<bool> SetParameterValueAsync(ParamKey key, bool? value, CancellationToken ct = default);
}

internal class DbParameterAccess(Db db, IMemoryCache cache) : IDbParameterAccess
{
    private IQueryable<DbParameter> BaseQuery(ParamKey key)
    {
        string name = cache.GetOrCreate(key, _ => key.GetDescription());
        return db.Parameters.Where(p => p.Name == name);
    }

    public Task<DbParameter?> GetParameterAsync(ParamKey key, CancellationToken ct = default)
    {
        return BaseQuery(key).FirstOrDefaultAsync(ct);
    }

    public Task<string?> GetStringParameterAsync(ParamKey key, CancellationToken ct = default)
    {
        return BaseQuery(key).Select(p => p.String).FirstOrDefaultAsync(ct);
    }

    public Task<DateTime?> GetDateTimeParameterAsync(ParamKey key, CancellationToken ct = default)
    {
        return BaseQuery(key).Select(p => p.DateTime).FirstOrDefaultAsync(ct);
    }

    public Task<TimeSpan?> GetTimeSpanParameterAsync(ParamKey key, CancellationToken ct = default)
    {
        return BaseQuery(key).Select(p => p.TimeSpan).FirstOrDefaultAsync(ct);
    }

    public Task<double?> GetNumericParameterAsync(ParamKey key, CancellationToken ct = default)
    {
        return BaseQuery(key).Select(p => p.Numeric).FirstOrDefaultAsync(ct);
    }

    public Task<bool?> GetBooleanParameterAsync(ParamKey key, CancellationToken ct = default)
    {
        return BaseQuery(key).Select(p => p.Boolean).FirstOrDefaultAsync(ct);
    }

    public async Task<bool> SetParameterValueAsync(ParamKey key, string? value, CancellationToken ct = default)
    {
        return await BaseQuery(key).ExecuteUpdateAsync(update => update.SetProperty(p => p.String, value), ct) > 0;
    }
    public async Task<bool> SetParameterValueAsync(ParamKey key, DateTime? value, CancellationToken ct = default)
    {
        return await BaseQuery(key).ExecuteUpdateAsync(update => update.SetProperty(p => p.DateTime, value), ct) > 0;
    }
    public async Task<bool> SetParameterValueAsync(ParamKey key, TimeSpan? value, CancellationToken ct = default)
    {
        return await BaseQuery(key).ExecuteUpdateAsync(update => update.SetProperty(p => p.TimeSpan, value), ct) > 0;
    }
    public async Task<bool> SetParameterValueAsync(ParamKey key, double? value, CancellationToken ct = default)
    {
        return await BaseQuery(key).ExecuteUpdateAsync(update => update.SetProperty(p => p.Numeric, value), ct) > 0;
    }
    public async Task<bool> SetParameterValueAsync(ParamKey key, bool? value, CancellationToken ct = default)
    {
        return await BaseQuery(key).ExecuteUpdateAsync(update => update.SetProperty(p => p.Boolean, value), ct) > 0;
    }
}
