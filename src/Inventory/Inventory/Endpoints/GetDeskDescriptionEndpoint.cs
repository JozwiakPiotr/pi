using FastEndpoints;
using Inventory.Api;
using Inventory.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Endpoints;

public sealed class GetDeskDescriptionEndpoint(InventoryDbContext dbContext) : EndpointWithoutRequest<DeskDto>
{
    public override void Configure()
    {
        Get("/desks/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
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

        await HttpContext.Response.SendAsync(desk.ToDto(), cancellation: ct);
    }
}
