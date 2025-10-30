using Sandbox;
using Sandbox.Diagnostics;

public enum Role
{
    Uninitialized,
    Hider,
    Seeker
}

public sealed class Player : Component, Component.ITriggerListener
{
    /* Public Properties */

    [Property, ReadOnly]
    [Sync(SyncFlags.FromHost)]
    [Change(nameof(OnRoleChanged))]
    public Role Role { get; set; } = Role.Uninitialized;

    [RequireComponent]
    Movement Movement { get; set; }

    [RequireComponent]
    Clothes Clothes { get; set; }
    
    [Property]
    Clothing HiderSuit { get; set; }
    
    [Property]
    Clothing SeekerSuit { get; set; }

    protected override void OnStart()
    {
        Role = Role.Hider;
    }

    void OnRoleChanged(Role _, Role role)
    {
        if (role == Role.Hider)
        {
            Clothes.WearSuit(HiderSuit);
        }
        else if (role == Role.Seeker)
        {
            Clothes.WearSuit(SeekerSuit);
        }
    }

    [ConCmd("hide", ConVarFlags.Server)]
    static void Hide(Connection caller)
    {
        if (Round.Instance.Stage != RoundStage.Playing)
        {
            caller.SendLog(LogLevel.Info, "You can't change your role at this moment.");
            return;
        }

        var player = GetOwnedBy(caller);
        player.Role = Role.Hider;
    }

    [ConCmd("seek", ConVarFlags.Server)]
    static void Seek(Connection caller)
    {
        if (Round.Instance.Stage != RoundStage.Playing)
        {
            caller.SendLog(LogLevel.Info, "You can't change your role at this moment.");
            return;
        }
        
        var player = GetOwnedBy(caller);
        player.Role = Role.Seeker;
    }

    public static List<Player> GetAll()
    {
        return Game.ActiveScene.GetAllComponents<Player>().ToList();
    }

    public static Player GetOwnedBy(Connection connection)
    {
        return GetAll().Find(p => p.Network.OwnerId == connection.Id);
    }

    public static Player GetLocal()
    {
        return GetOwnedBy(Connection.Local);
    }

    public static Player GetHost()
    {
        return GetOwnedBy(Connection.Host);
    }

    public static Player GetRandom()
    {
        return Game.Random.FromList(GetAll());
    }

    public void Teleport(Vector3 worldPosition) => Movement.Teleport(worldPosition);
    public void Freeze(bool freeze) => Movement.Freeze(freeze);
}