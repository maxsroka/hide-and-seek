using Sandbox;
namespace HNS;

public class Role : Component
{
    [Sync(SyncFlags.FromHost)]
    [Change(nameof(OnRoleChanged))]
    public BaseRole Current { get; private set; }

	public void Set<T>() where T : BaseRole
	{
		Current = GetComponent<T>(includeDisabled: true);
	}

	void OnRoleChanged(BaseRole oldRole, BaseRole newRole)
    {
        if (oldRole != null)
        {
            oldRole.Enabled = false;
        }

        newRole.Enabled = true;
    }

    [ConCmd("hide", ConVarFlags.Server)]
    static void Hide(Connection caller) => Player.GetOwnedBy(caller).SetRole<HiderRole>();

    [ConCmd("seek", ConVarFlags.Server)]
    static void Seek(Connection caller) => Player.GetOwnedBy(caller).SetRole<SeekerRole>();
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
	[RequireComponent]
	Player Player { get; set; }

    void ITriggerListener.OnTriggerEnter(Collider other)
    {
		if (!Networking.IsHost) return;
		if (Player.IsFrozen) return;
		
		TryTag(other);
    }

    void TryTag(Collider other)
    {
        var otherPlayer = other.GetComponent<Player>();
        if (otherPlayer == null) return;
		if (otherPlayer.CurrentRole is not HiderRole) return;

		otherPlayer.SetRole<SeekerRole>();
		Chat.SystemMessage($"{Player.Network.Owner.DisplayName} has caught {otherPlayer.Network.Owner.DisplayName}");
    }
}

public class HiderRole : BaseRole
{
    
}
