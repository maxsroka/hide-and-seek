using Sandbox;

public class Movement : Component
{
    [Property]
    CapsuleCollider standingTrigger;

    [Property]
    CapsuleCollider duckingTrigger;

    [RequireComponent]
    PlayerController Controller { get; set; }

    [Rpc.Owner(NetFlags.HostOnly)]
    public void Teleport(Vector3 worldPosition)
    {
        WorldPosition = worldPosition;
    }

    [Rpc.Owner(NetFlags.HostOnly)]
    public void Freeze(bool freeze)
    {
        Controller.UseInputControls = !freeze;
        Controller.WishVelocity = Vector3.Zero;
        GUI.Instance.BlindScreen.Toggle(freeze);
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
            standingTrigger.Enabled = false;
            duckingTrigger.Enabled = true;
        }
        else
        {
            standingTrigger.Enabled = true;
            duckingTrigger.Enabled = false;
        }
    }
}