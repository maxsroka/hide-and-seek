using Sandbox;

public sealed class PlayerRole : Component
{
    [Property, ReadOnly]
    [Sync(SyncFlags.FromHost)]
    [Change(nameof(OnRoleChanged))]
    Role Role { get; set; } = Role.Uninitialized;

	protected override void OnStart()
	{
        Role = Role.Hider;
	}

    void OnRoleChanged(Role _, Role role)
    {
        if (role == Role.Hider)
        {
            IRoleEvent.PostToGameObject(GameObject, e => e.OnHider());
        }
        else if (role == Role.Seeker)
        {
            IRoleEvent.PostToGameObject(GameObject, e => e.OnSeeker());
        }
    }
    
    [ConCmd("hide", ConVarFlags.Server)]
    static void Hide(Connection caller)
    {
        var player = PlayerFinder.OwnedBy(caller);
        var playerRole = player.GetComponent<PlayerRole>();
        playerRole.Role = Role.Hider;
    }
    
    [ConCmd("seek", ConVarFlags.Server)]
    static void Seek(Connection caller)
    {
        var player = PlayerFinder.OwnedBy(caller);
        var playerRole = player.GetComponent<PlayerRole>();
        playerRole.Role = Role.Seeker;
    }
}