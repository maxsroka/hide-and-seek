using Sandbox;

public sealed class TouchModule : Component, Component.ITriggerListener
{
    [Property]
    CapsuleCollider standingCollider;
    
    [Property]
    CapsuleCollider duckingCollider;

    public void OnTriggerEnter(Collider other)
    {
        if (!Connection.Local.IsHost) return;
        if (Player.GetHost().Role != Role.Seeker) return;
        if (Round.Instance.Stage != RoundStage.Playing) return;

        var otherPlayer = other.GetComponent<Player>();
        if (otherPlayer == null) return;
        if (otherPlayer.Role != Role.Hider) return;

        otherPlayer.Role = Role.Seeker;
    }

	protected override void OnFixedUpdate()
    {
        if (!Connection.Local.IsHost) return;

        var player = Player.GetHost();

        if (player.Controller.IsDucking && !player.Controller.IsAirborne)
        {
            standingCollider.Enabled = false;
            duckingCollider.Enabled = true;
        }
        else
        {
            standingCollider.Enabled = true;
            duckingCollider.Enabled = false;
        }
	}
}