using Sandbox;
namespace HNS;

public sealed class Freeze : Component
{
	[Sync(SyncFlags.FromHost)]
	[Change(nameof(OnFrozen))]
	public bool IsFrozen { get; set; } = false;

	[RequireComponent]
	PlayerController Controller { get; set; }

	void OnFrozen(bool _, bool isFrozen)
	{
		if (!Network.IsOwner) return;

		Controller.UseInputControls = !isFrozen;
		Controller.WishVelocity = Vector3.Zero;
	}
}
