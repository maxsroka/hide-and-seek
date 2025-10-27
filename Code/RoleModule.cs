using Sandbox;

public enum Role
{
    Uninitialized,
    Hider,
    Seeker
}

public interface IRoleEvent : ISceneEvent<IRoleEvent>
{
    void OnHider() { }
    void OnSeeker() { }
}

public sealed class RoleModule : Component
{
    [Property, ReadOnly]
    [Sync(SyncFlags.FromHost)]
    [Change(nameof(OnRoleChanged))]
    public Role Role { get; set; } = Role.Uninitialized;

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
        var player = Player.GetOwnedBy(caller);
        player.Role = Role.Hider;
    }

    [ConCmd("seek", ConVarFlags.Server)]
    static void Seek(Connection caller)
    {
        var player = Player.GetOwnedBy(caller);
        player.Role = Role.Seeker;
    }
}