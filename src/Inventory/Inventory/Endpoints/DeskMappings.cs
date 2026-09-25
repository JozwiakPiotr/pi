using Inventory.Api;
using Inventory.Domain;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Endpoints;

internal static class DeskMappings
{
    public static IQueryable<Desk> WithDescription(this IQueryable<Desk> query)
    {
        return query
            .Include(x => x.Room)
            .Include(x => x.Features)
            .ThenInclude(x => x.Feature)
            .Include(x => x.Assets)
            .ThenInclude(x => x.Asset);
    }

    public static DeskDto ToDto(this Desk desk)
    {
        var features = desk.Features
            .Select(x => new DeskFeatureDto(x.FeatureId, x.Feature.Name))
            .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var assets = desk.Assets
            .Select(x => new DeskAssetDto(x.AssetId, x.Asset.Name, x.Count))
            .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new DeskDto(
            desk.Id,
            desk.Number,
            desk.State,
            new RoomDto(desk.RoomId, desk.Room.Number, desk.Room.Level),
            features,
            assets);
    }

    public static string NormalizeName(string value) => value.Trim();
}
