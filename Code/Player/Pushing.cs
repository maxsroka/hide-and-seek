using Sandbox;
namespace HNS;

public class Pushing : Component
{
	[ConVar("push_strength", ConVarFlags.Replicated)]
	static float Strength { get; set; } = 500f;

	[RequireComponent]
	PlayerController Controller { get; set; }

	[RequireComponent]
	Player Player { get; set; }

	[Rpc.Owner]
	void Push(Vector3 direction)
	{
		Controller.Jump(direction * Strength);
	}

	protected override void OnUpdate()
	{
		if (!Network.IsOwner) return;

		if (Input.Pressed("use"))
		{
			var trace = Player.Trace;
			if (trace.Hit && trace.GameObject.Components.TryGet(out Pushing pushing))
			{
				pushing.Push(trace.Direction);
			}
		}
	}
}
