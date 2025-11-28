using Sandbox;
namespace HNS;

public class Ducking : Component
{
	[Property]
	CapsuleCollider StandingTrigger { get; set; }

	[Property]
	CapsuleCollider DuckingTrigger { get; set; }

	[RequireComponent]
	PlayerController Controller { get; set; }

	protected override void OnUpdate()
	{
		if (!Networking.IsHost) return;

		AdjustTriggers();
	}

	void AdjustTriggers()
	{
		if (Controller.IsDucking && !Controller.IsAirborne)
		{
			StandingTrigger.Enabled = false;
			DuckingTrigger.Enabled = true;
		}
		else
		{
			StandingTrigger.Enabled = true;
			DuckingTrigger.Enabled = false;
		}
	}
}
