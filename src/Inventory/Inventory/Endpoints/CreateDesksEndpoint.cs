using FastEndpoints;
using Inventory.Api;
using Inventory.Data;
using Inventory.Domain;
using Inventory.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Inventory.Endpoints;

public sealed class CreateDesksEndpoint(InventoryDbContext dbContext, IMessageBus messageBus)
    : Endpoint<CreateDeskBatchRequest, IReadOnlyCollection<DeskDto>>
{
    public override void Configure()
    {
        Post("/desks/batch");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateDeskBatchRequest req, CancellationToken ct)
    {
        if (req.Desks.Count == 0)
        {
            AddError("desks", "At least one desk must be provided.");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var createdDeskIds = new List<Guid>(req.Desks.Count);

        foreach (var request in req.Desks)
        {
            var room = await GetOrCreateRoomAsync(request.Room, ct);

            var number = request.Number.Trim();
            var desk = new Desk(number, room);

            var features = await ResolveFeaturesAsync(request.Features, ct);
            var assets = await ResolveAssetsAsync(request.Assets, ct);
            desk.UpdateEquipment(features, assets);

            dbContext.Desks.Add(desk);
            createdDeskIds.Add(desk.Id);
        }

        await dbContext.SaveChangesAsync(ct);

        var desks = await dbContext.Desks
            .WithDescription()
            .Where(x => createdDeskIds.Contains(x.Id))
            .ToListAsync(ct);

        foreach (var desk in desks)
        {
            await messageBus.PublishAsync(new DeskAdded(
                desk.Id,
                desk.Number,
                desk.Room.Number,
                desk.Room.Level));
        }

        await HttpContext.Response.SendAsync(desks.Select(x => x.ToDto()).ToArray(), 201, null, ct);
    }

    private async Task<Room> GetOrCreateRoomAsync(RoomInput input, CancellationToken ct)
    {
        var number = input.Number.Trim();
        var room = await dbContext.Rooms
            .FirstOrDefaultAsync(x => x.Number == number && x.Level == input.Level, ct);

        if (room is not null)
        {
            return room;
        }

        room = new Room(number, input.Level);
        dbContext.Rooms.Add(room);
        return room;
    }

    private async Task<IReadOnlyCollection<Feature>> ResolveFeaturesAsync(IReadOnlyCollection<string>? names, CancellationToken ct)
    {
        if (names is null || names.Count == 0)
        {
            return [];
        }

        var normalizedNames = names
            .Select(DeskMappings.NormalizeName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalizedNames.Length == 0)
        {
            return [];
        }

        var existingList = await dbContext.Features
            .Where(x => normalizedNames.Contains(x.Name))
            .ToListAsync(ct);

        var existing = existingList.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var name in normalizedNames)
        {
            if (existing.ContainsKey(name))
            {
                continue;
            }

            var feature = new Feature(name);
            dbContext.Features.Add(feature);
            existing[name] = feature;
        }

        return normalizedNames.Select(name => existing[name]).ToArray();
    }

    private async Task<IReadOnlyCollection<(Asset Asset, int Count)>> ResolveAssetsAsync(
        IReadOnlyCollection<DeskAssetInput>? assets,
        CancellationToken ct)
    {
        if (assets is null || assets.Count == 0)
        {
            return [];
        }

        var aggregated = assets
            .Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.Count > 0)
            .GroupBy(x => DeskMappings.NormalizeName(x.Name), StringComparer.OrdinalIgnoreCase)
            .Select(x => new { Name = x.Key, Count = x.Sum(a => a.Count) })
            .ToArray();

        if (aggregated.Length == 0)
        {
            return [];
        }

        var assetNames = aggregated.Select(x => x.Name).ToArray();
        var existingList = await dbContext.Assets
            .Where(x => assetNames.Contains(x.Name))
            .ToListAsync(ct);

        var existing = existingList.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var item in aggregated)
        {
            if (existing.ContainsKey(item.Name))
            {
                continue;
            }

            var asset = new Asset(item.Name);
            dbContext.Assets.Add(asset);
            existing[item.Name] = asset;
        }

        return aggregated
            .Select(x => (existing[x.Name], x.Count))
            .ToArray();
    }
}