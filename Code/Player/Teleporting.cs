using Sandbox;
namespace HNS;

public class Teleporting : Component
{
	[Rpc.Owner(NetFlags.HostOnly)]
	public void Teleport(Vector3 worldPosition)
	{
		WorldPosition = worldPosition;
	}
}
