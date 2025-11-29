using Sandbox;
namespace HNS;

public class Player : Component
{
    [RequireComponent] Role Role { get; set; }
    [RequireComponent] Clothes Clothes { get; set; }
    [RequireComponent] Stamina Stamina { get; set; }
    [RequireComponent] Freezing Freezing { get; set; }
    [RequireComponent] Teleporting Teleporting { get; set; }

	public static List<Player> GetAll() => Game.ActiveScene.GetAllComponents<Player>().ToList();
    public static Player GetOwnedBy(Connection connection) => GetAll().Find(p => p.Network.OwnerId == connection.Id);
    public static Player GetLocal() => GetOwnedBy(Connection.Local);
    public static Player GetHost() => GetOwnedBy(Connection.Host);
    public static Player GetRandom() => Game.Random.FromList(GetAll());

	public BaseRole CurrentRole => Role.Current;
	public void SetRole<T>() where T : BaseRole => Role.Set<T>();

	public float CurrentStamina => Stamina.Current;
	public float MaxStamina => Stamina.Max;

	public bool IsFrozen { get => Freezing.IsFrozen; set => Freezing.IsFrozen = value; }
	
	public void Teleport(Vector3 worldPosition) => Teleporting.Teleport(worldPosition);
	
	public void Equip(Clothing suit) => Clothes.Equip(suit);
	
	[System.Obsolete]
    public bool IsSeeker => Role.Current is SeekerRole;
	[System.Obsolete]
    public bool IsHider => Role.Current is HiderRole;

	public interface ISpawnListener : ISceneEvent<ISpawnListener>
	{
		void OnSpawned(Connection connection);
	}

	protected override void OnStart()
	{
		if (!Networking.IsHost) return;

		ISpawnListener.Post(e => e.OnSpawned(Network.Owner));
	}
}
