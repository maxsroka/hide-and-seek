using Sandbox;
using Sandbox.Movement;
using System;
namespace HNS;

public interface IMovementEvents : ISceneEvent<IMovementEvents>
{
    void OnFreeze(bool isFrozen);
}

public class Movement : Component
{
	public bool IsFrozen { get; private set; } = false;

	[Property]
    CapsuleCollider StandingTrigger { get; set; }

    [Property]
    CapsuleCollider DuckingTrigger { get; set; }

    [RequireComponent]
    PlayerController Controller { get; set; }

	[Rpc.Owner(NetFlags.HostOnly)]
    public void Teleport(Vector3 worldPosition)
    {
        WorldPosition = worldPosition;
    }

    public void Freeze(bool freeze)
    {
		IsFrozen = freeze;
		FreezeOnOwner(freeze);
    }

    [Rpc.Owner(NetFlags.HostOnly)]
	void FreezeOnOwner(bool freeze)
	{
        Controller.UseInputControls = !freeze;
        Controller.WishVelocity = Vector3.Zero;
		IMovementEvents.Post(e => e.OnFreeze(freeze));
	}

    protected override void OnFixedUpdate()
    {
        if (Connection.Local.IsHost)
        {
            AdjustTriggers();
        }
    }
    
    void AdjustTriggers()
    {
        if (Controller.IsDucking && !Controller.IsAirborne)
        {
            StandingTrigger.Enabled = false;
            DuckingTrigger.Enabled = true;
        }
        else
        {
            StandingTrigger.Enabled = true;
            DuckingTrigger.Enabled = false;
        }
    }
}
