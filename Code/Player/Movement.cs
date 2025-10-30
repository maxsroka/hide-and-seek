using Sandbox;

public class Movement : Component
{
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
}