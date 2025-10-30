using Sandbox;

public class Player : Component
{
    [RequireComponent] Movement Movement { get; set; }
    [RequireComponent] Clothes Clothes { get; set; }
    [RequireComponent] Role Role { get; set; }

    public static List<Player> GetAll() => Game.ActiveScene.GetAllComponents<Player>().ToList();
    public static Player GetOwnedBy(Connection connection) => GetAll().Find(p => p.Network.OwnerId == connection.Id);
    public static Player GetLocal() => GetOwnedBy(Connection.Local);
    public static Player GetHost() => GetOwnedBy(Connection.Host);
    public static Player GetRandom() => Game.Random.FromList(GetAll());

    public void Teleport(Vector3 worldPosition) => Movement.Teleport(worldPosition);
    public void Freeze(bool freeze) => Movement.Freeze(freeze);
    public void Seek() => Role.Seek();
    public void Hide() => Role.Hide();
    public bool IsSeeker => Role.IsSeeker;
    public bool IsHider => Role.IsHider;
    public void Equip(Clothing clothing) => Clothes.Equip(clothing);
}