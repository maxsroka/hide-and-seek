using Sandbox;

public sealed class PlayerTrigger : Component, Component.ITriggerListener
{
    [RequireComponent]
    PlayerRole PlayerRole { get; set; }

    public void OnTriggerEnter(Collider other)
    {
        if (!Connection.Local.IsHost) return;
        if (PlayerRole.Role != Role.Seeker) return;
        if (Round.Instance.Stage != RoundStage.Playing) return;

        var otherPlayerRole = other.GetComponent<PlayerRole>();
        if (otherPlayerRole == null) return;

        if (otherPlayerRole.Role != Role.Hider) return;

        otherPlayerRole.Role = Role.Seeker;
    }
}