using Sandbox;

public sealed class TouchModule : Component, Component.ITriggerListener
{
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
}