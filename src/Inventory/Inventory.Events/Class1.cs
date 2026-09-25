namespace Inventory.Events;

public sealed record DeskAdded(
	Guid DeskId,
	string Number,
	string RoomNumber,
	int RoomLevel);

public sealed record DeskRemoved(Guid DeskId);

public sealed record DeskAssetSnapshot(
	string Name,
	int Count);

public sealed record DeskEquipmentUpdated(
	Guid DeskId,
	IReadOnlyCollection<DeskAssetSnapshot> Assets,
	IReadOnlyCollection<string> Features);

public sealed record DeskStatusChanged(
	Guid DeskId,
	string Number,
	string State);
