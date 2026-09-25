using FastEndpoints;
using Inventory.Api;
using Inventory.Data;
using Inventory.Domain;
using Inventory.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Inventory.Endpoints;

public sealed class UpdateDeskEquipmentEndpoint(InventoryDbContext dbContext, IMessageBus messageBus)
    : Endpoint<UpdateDeskEquipmentRequest, DeskDto>
{
    public override void Configure()
    {
        Put("/desks/{id:guid}/equipment");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateDeskEquipmentRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var desk = await dbContext.Desks
            .WithDescription()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (desk is null)
        {
            await HttpContext.Response.SendNotFoundAsync(ct);
            return;
        }

        var features = await ResolveFeaturesAsync(req.Features, ct);
        var assets = await ResolveAssetsAsync(req.Assets, ct);

        desk.UpdateEquipment(features, assets);
        await dbContext.SaveChangesAsync(ct);

        await messageBus.PublishAsync(new DeskEquipmentUpdated(
            desk.Id,
            desk.Assets.Select(x => new DeskAssetSnapshot(x.Asset.Name, x.Count)).ToArray(),
            desk.Features.Select(x => x.Feature.Name).ToArray()));

        var response = desk.ToDto();
        await HttpContext.Response.SendAsync(response, cancellation: ct);
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