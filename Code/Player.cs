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

    /* Components */

    [RequireComponent]
    PlayerController Controller { get; set; }

    [RequireComponent]
    Dresser Dresser { get; set; }
    
    /* Private Properties */

    [Property, Group("Trigger")]
    CapsuleCollider StandingCollider { get; set; }

    [Property, Group("Trigger")]
    CapsuleCollider DuckingCollider { get; set; }
    
    [Property, Group("Clothing")]
    Clothing.Slots SlotsFilter { get; set; }

    [Property, Group("Clothing")]
    Clothing HiderSuit { get; set; }
    
    [Property, Group("Clothing")]
    Clothing SeekerSuit { get; set; }

    /* Component Events */

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
            StandingCollider.Enabled = false;
            DuckingCollider.Enabled = true;
        }
        else
        {
            StandingCollider.Enabled = true;
            DuckingCollider.Enabled = false;
        }
    }

    /* Collider Events */
    
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

    /* Other Events */

    void OnRoleChanged(Role _, Role role)
    {
        if (role == Role.Hider)
        {
            WearSuit(HiderSuit);
        }
        else if (role == Role.Seeker)
        {
            WearSuit(SeekerSuit);
        }
    }

    /* Commands */

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

    /* Query Methods */

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
        Controller.WishVelocity = Vector3.Zero;
    }

    [Rpc.Owner(NetFlags.HostOnly)]
    public void Blind(bool blind)
    {
        GUI.Instance.BlindScreen.Toggle(blind);
    }

    // Private Methods

    void WearSuit(Clothing suit)
    {
        var userClothingContainer = GetUserClothingContainer();
        var newClothingContainer = FilterClothingContainer(userClothingContainer, SlotsFilter);

        newClothingContainer.Add(suit);
        
        Dresser.Clothing = newClothingContainer.Clothing;
        Dresser.Apply();
    }

    ClothingContainer FilterClothingContainer(ClothingContainer original, Clothing.Slots filter)
    {
        var filtered = new ClothingContainer();

        foreach (var entry in original.Clothing)
        {
            if ((entry?.Clothing?.SlotsUnder & filter) != 0) continue;

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