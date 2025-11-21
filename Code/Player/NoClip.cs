using Sandbox;
using Sandbox.Movement;
namespace HNS;

public class NoClip : MoveMode
{
	const float SPEED = 1000f;

	bool toggled = false;

	[ConCmd("noclip", ConVarFlags.Cheat)]
	static void Toggle(Connection caller)
	{
		var player = Player.GetOwnedBy(caller);
		var noClip = player.GetComponent<NoClip>();
		noClip.toggled = !noClip.toggled;
	}

	public override int Score(PlayerController controller)
	{
		return toggled ? 1000 : -1000;
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
		Controller.Body.Velocity = Controller.WishVelocity.Normal * SPEED;
	}
}
