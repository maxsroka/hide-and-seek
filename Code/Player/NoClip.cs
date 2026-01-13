using Sandbox;
using Sandbox.Movement;
namespace HNS;

public class NoClip : MoveMode
{
	[ConVar("noclip_speed", ConVarFlags.Replicated, Help = "Set the flying speed.")]
	public static float Speed { get; set; } = 1000f;

	bool isOn = false;

	[ConCmd("noclip", ConVarFlags.Cheat, Help = "Toggle flying on and off.")]
	static void Toggle(Connection caller)
	{
		var player = Player.GetOwnedBy(caller);
		var noClip = player.GetComponent<NoClip>();
		noClip.isOn = !noClip.isOn;
	}

	public override int Score(PlayerController controller)
	{
		return isOn ? 1000 : -1000;
	}

	public override void OnModeBegin()
	{
		Controller.ColliderObject.Enabled = false;
	}

	public override void OnModeEnd(MoveMode next)
	{
		Controller.ColliderObject.Enabled = true;
	}

	public override void UpdateRigidBody(Rigidbody body)
	{
		body.Gravity = false;
		body.LinearDamping = 3.3f;
		body.AngularDamping = 1f;
	}

	public override Vector3 UpdateMove(Rotation eyes, Vector3 input)
	{
		if (Input.Down("jump"))
		{
			input += Vector3.Up;
		}

		if (Input.Down("duck"))
		{
			input += Vector3.Down;
		}

		return base.UpdateMove(eyes, input);
	}

	public override void AddVelocity()
	{
		Controller.Body.Velocity = Controller.WishVelocity.Normal * Speed;
	}
}
