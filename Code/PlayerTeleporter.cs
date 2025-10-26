using Sandbox;

public sealed class PlayerTeleporter : Component
{
    [Rpc.Owner(NetFlags.HostOnly)]
    public void Teleport(Vector3 worldPosition)
    {
        WorldPosition = worldPosition;
    }
}
