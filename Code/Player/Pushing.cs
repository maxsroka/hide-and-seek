using Sandbox;
namespace HNS;

public class Pushing : Component
{
	[ConVar("push_strength", ConVarFlags.Replicated, Help = "Set how far players are pushed away.")]
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
		if (Player.IsFrozen) return;

		if (Input.Pressed("use"))
		{
			var trace = Player.Trace;
			if (trace.Hit && trace.GameObject.Components.TryGet(out Player pushedPlayer))
			{
				if (pushedPlayer.IsFrozen) return;

				var pushing = pushedPlayer.GetComponent<Pushing>();
				pushing.Push(trace.Direction);
			}
		}
	}
}
