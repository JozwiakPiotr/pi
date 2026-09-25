namespace Inventory.Domain;

public sealed class DeskFeature
{
    private DeskFeature()
    {
    }

    public DeskFeature(Guid deskId, Guid featureId)
    {
        DeskId = deskId;
        FeatureId = featureId;
    }

    public Guid DeskId { get; private set; }

    public Desk Desk { get; private set; } = null!;

    public Guid FeatureId { get; private set; }

    public Feature Feature { get; private set; } = null!;
}
