using Sandbox;
using Sandbox.Diagnostics;

public sealed class Player : Component
{
    [RequireComponent]
    RoleModule RoleModule { get; set; }

    [RequireComponent]
    public PlayerController Controller { get; private set; }

    public Role Role
    {
        get => RoleModule.Role;
        set => RoleModule.Role = value;
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

    [Rpc.Owner(NetFlags.HostOnly)]
    public void Teleport(Vector3 worldPosition)
    {
        WorldPosition = worldPosition;
    }
}