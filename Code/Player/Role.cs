using Sandbox;

public interface IRoleEvents : ISceneEvent<IRoleEvents>
{
    void OnRole(string name);
}

public class Role : Component
{
    [Sync(SyncFlags.FromHost)]
    [Change(nameof(OnRoleChanged))]
    BaseRole Current { get; set; }

    [RequireComponent]
    SeekerRole Seeker { get; set; }

    [RequireComponent]
    HiderRole Hider { get; set; }

    public void Seek() => Current = Seeker;
    public void Hide() => Current = Hider;
    public bool IsSeeker => Current == Seeker;
    public bool IsHider => Current == Hider;

    void OnRoleChanged(BaseRole oldRole, BaseRole newRole)
    {
        if (oldRole != null)
        {
            oldRole.Enabled = false;
        }

        newRole.Enabled = true;
        IRoleEvents.Post(e => e.OnRole(newRole.Name));
    }

    protected override void OnStart()
    {
        if (Connection.Local.IsHost)
        {
            Hide();
        }
    }
    
    [ConCmd("hide", ConVarFlags.Server)]
    static void Hide(Connection caller) => Player.GetOwnedBy(caller).Hide();

    [ConCmd("seek", ConVarFlags.Server)]
    static void Seek(Connection caller) => Player.GetOwnedBy(caller).Seek();
}

public abstract class BaseRole : Component
{
    [Property]
    public string Name { get; set; }

    [Property]
    public Clothing Suit { get; set; }

	protected override void OnEnabled()
    {
        GetComponent<Player>().Equip(Suit);
    }
}

public class SeekerRole : BaseRole, Component.ITriggerListener
{
    void ITriggerListener.OnTriggerEnter(Collider other)
    {
        if (Connection.Local.IsHost)
        {
            TryTag(other);
        }
    }

    void TryTag(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player == null || !player.IsHider) return;

        player.Seek();
    }
}

public class HiderRole : BaseRole
{
    
}
