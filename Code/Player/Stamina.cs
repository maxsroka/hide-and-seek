using Sandbox;
using System;
namespace HNS;

public class Stamina : Component
{
	public float Max { get; set; } = 5f;
	public float Current { get; private set; }

	[RequireComponent]
	PlayerController Controller { get; set; }

	protected override void OnStart()
	{
		Current = Max;
	}

	protected override void OnUpdate()
	{
		if (!Network.IsOwner) return;

		if (Input.Down("run"))
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
