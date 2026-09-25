namespace Inventory.Api;

public enum DeskState
{
	Available = 0,
	OutOfService = 1
}

public sealed record RoomDto(
	Guid Id,
	string Number,
	int Level);

public sealed record DeskFeatureDto(
	Guid Id,
	string Name);

public sealed record DeskAssetDto(
	Guid Id,
	string Name,
	int Count);

public sealed record DeskDto(
	Guid Id,
	string Number,
	DeskState State,
	RoomDto Room,
	IReadOnlyCollection<DeskFeatureDto> Features,
	IReadOnlyCollection<DeskAssetDto> Assets);

public sealed record RoomInput(
	string Number,
	int Level);

public sealed record DeskAssetInput(
	string Name,
	int Count);

public sealed record CreateDeskRequest(
	string Number,
	RoomInput Room,
	IReadOnlyCollection<string>? Features,
	IReadOnlyCollection<DeskAssetInput>? Assets);

public sealed record CreateDeskBatchRequest(IReadOnlyCollection<CreateDeskRequest> Desks);

public sealed record UpdateDeskEquipmentRequest(
	IReadOnlyCollection<string>? Features,
	IReadOnlyCollection<DeskAssetInput>? Assets);

public sealed record ChangeDeskStatusRequest(DeskState State);
