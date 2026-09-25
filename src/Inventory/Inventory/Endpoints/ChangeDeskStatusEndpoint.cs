using FastEndpoints;
using Inventory.Api;
using Inventory.Data;
using Inventory.Domain;
using Inventory.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Inventory.Endpoints;

public sealed class ChangeDeskStatusEndpoint(InventoryDbContext dbContext, IMessageBus messageBus)
    : Endpoint<ChangeDeskStatusRequest, DeskDto>
{
    public override void Configure()
    {
        Put("/desks/{id:guid}/status");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ChangeDeskStatusRequest req, CancellationToken ct)
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

        desk.SetState(req.State);
        await dbContext.SaveChangesAsync(ct);

        await messageBus.PublishAsync(new DeskStatusChanged(desk.Id, desk.Number, desk.State.ToString()));

        var response = desk.ToDto();
        await HttpContext.Response.SendAsync(response, cancellation: ct);
    }
}