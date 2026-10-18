using KrzaqRPG.Backend.Common.Enums;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.TravelEvents;

public interface IDbTravelEventAccess
{
    Task<int> AddTravelPathEventAsync(DateTime timestamp, EventType type, int travelId, int travelPathId, CancellationToken ct = default);
    Task<int> AddTravelAreaEventAsync(DateTime timestamp, EventType type, int travelId, int areaId, CancellationToken ct = default);
}

internal class DbTravelEventAccess(Db db) : IDbTravelEventAccess
{
    public async Task<int> AddTravelPathEventAsync(DateTime timestamp, EventType type, int travelId, int travelPathId, CancellationToken ct = default)
    {
        var @event = new DbTravelEvent
        {
            Timestamp = timestamp,
            Type = type,
            TravelId = travelId,
            TravelPathId = travelPathId,
        };

        _ = await db.TravelEvents.AddAsync(@event, ct);

        return @event.Id;
    }

    public async Task<int> AddTravelAreaEventAsync(DateTime timestamp, EventType type, int travelId, int areaId, CancellationToken ct = default)
    {
        var @event = new DbTravelEvent
        {
            Timestamp = timestamp,
            Type = type,
            TravelId = travelId,
            AreaId = areaId,
        };

        _ = await db.TravelEvents.AddAsync(@event, ct);

        return @event.Id;
    }
}
