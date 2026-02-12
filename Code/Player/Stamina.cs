using Sandbox;
using System;
namespace HNS;

public class Stamina : Component
{
	[ConVar("max_stamina", ConVarFlags.Replicated, Help = "Set the maximum sprint duration.", Min = 0)]
	public static float Max { get; set; } = 5f;
	
	public float Current { get; private set; } = 0f;
	public bool IsSprinting { get; private set; } = false;

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
			IsSprinting = true;
			Current = MathF.Max(0, Current - Time.Delta);
		}
		else
		{
			IsSprinting = false;
			Current = Math.Min(Max, Current + Time.Delta);
		}

		Controller.AltMoveButton = Current > 0f ? "run" : null;
	}
}
