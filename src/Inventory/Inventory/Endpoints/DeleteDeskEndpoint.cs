using FastEndpoints;
using Inventory.Api;
using Inventory.Data;
using Inventory.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Inventory.Endpoints;

public sealed record DeleteDeskRequest(Guid Id);

public sealed class DeleteDeskEndpoint(InventoryDbContext dbContext, IMessageBus messageBus)
    : Endpoint<DeleteDeskRequest, DeskDto>
{
    public override void Configure()
    {
        Delete("/desks/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteDeskRequest req, CancellationToken ct)
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

        var response = desk.ToDto();

        dbContext.Desks.Remove(desk);
        await dbContext.SaveChangesAsync(ct);

        await messageBus.PublishAsync(new DeskRemoved(response.Id));

        await HttpContext.Response.SendAsync(response, cancellation: ct);
    }
}