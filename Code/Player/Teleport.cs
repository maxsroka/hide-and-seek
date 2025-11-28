using Sandbox;
namespace HNS;

public class Teleport : Component
{
	[Rpc.Owner(NetFlags.HostOnly)]
	public void TeleportTo(Vector3 worldPosition)
	{
		WorldPosition = worldPosition;
	}
}
