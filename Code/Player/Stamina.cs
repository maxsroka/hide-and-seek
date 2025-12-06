using Sandbox;
using System;
namespace HNS;

public class Stamina : Component
{
	[ConVar("max_stamina", ConVarFlags.Replicated)]
	public static float Max { get; set; } = 5f;
	
	public float Current { get; private set; } = 0f;

	[RequireComponent]
	PlayerController Controller { get; set; }

	[RequireComponent]
	Player Player { get; set; }

	protected override void OnStart()
	{
		if (!Network.IsOwner) return;

		Current = Max;
	}

	protected override void OnUpdate()
	{
		if (!Network.IsOwner) return;

		if (Input.Down("run") && !Player.IsFrozen)
		{
			Current = MathF.Max(0, Current - Time.Delta);
		}
		else
		{
			Current = Math.Min(Max, Current + Time.Delta);
		}

		Controller.AltMoveButton = Current > 0f ? "run" : null;
	}
}
