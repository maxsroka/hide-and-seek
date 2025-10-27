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
    // Public Properties

    [Property, ReadOnly]
    [Sync(SyncFlags.FromHost)]
    [Change(nameof(OnRoleChanged))]
    public Role Role { get; set; } = Role.Uninitialized;
    
    // Private Properties

    [RequireComponent]
    PlayerController Controller { get; set; }

    [RequireComponent]
    Dresser Dresser { get; set; }

    [Property]
    CapsuleCollider standingCollider;

    [Property]
    CapsuleCollider duckingCollider;
    
    [Property]
    Clothing.Slots slotsFilter;

    [Property]
    Clothing hiderSuit;
    
    [Property]
    Clothing seekerSuit;

    // Events

    protected override void OnStart()
    {
        Role = Role.Hider;
    }

    protected override void OnFixedUpdate()
    {
        if (!Connection.Local.IsHost) return;

        var player = GetHost();

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
    
    public void OnTriggerEnter(Collider other)
    {
        if (!Connection.Local.IsHost) return;
        if (GetHost().Role != Role.Seeker) return;
        if (Round.Instance.Stage != RoundStage.Playing) return;

        var otherPlayer = other.GetComponent<Player>();
        if (otherPlayer == null) return;
        if (otherPlayer.Role != Role.Hider) return;

        otherPlayer.Role = Role.Seeker;
    }

    void OnRoleChanged(Role _, Role role)
    {
        if (role == Role.Hider)
        {
            WearSuit(hiderSuit);
        }
        else if (role == Role.Seeker)
        {
            WearSuit(seekerSuit);
        }
    }

    // Commands

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

    // Querying

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

    // Public Methods

    [Rpc.Owner(NetFlags.HostOnly)]
    public void Teleport(Vector3 worldPosition)
    {
        WorldPosition = worldPosition;
    }

    [Rpc.Owner(NetFlags.HostOnly)]
    public void Freeze(bool freeze)
    {
        Controller.UseInputControls = !freeze;
    }

    // Private Methods

    void WearSuit(Clothing suit)
    {
        var userClothingContainer = GetUserClothingContainer();
        var newClothingContainer = FilterClothingContainer(userClothingContainer, slotsFilter);

        newClothingContainer.Add(suit);
        
        Dresser.Clothing = newClothingContainer.Clothing;
        Dresser.Apply();
    }

    ClothingContainer FilterClothingContainer(ClothingContainer original, Clothing.Slots filter)
    {
        var filtered = new ClothingContainer();

        foreach (var entry in original.Clothing)
        {
            if ((entry.Clothing.SlotsUnder & filter) != 0) continue;

            filtered.Add(entry);
        }

        return filtered;
    }
    
    ClothingContainer GetUserClothingContainer()
    {
        var container = new ClothingContainer();
        container.Deserialize(Network.Owner.GetUserData("avatar"));
        return container;
    }
}