using Sandbox;
namespace HNS;

public class Player : Component
{
    [RequireComponent] Role Role { get; set; }
    [RequireComponent] Clothes Clothes { get; set; }
    [RequireComponent] Stamina Stamina { get; set; }
    [RequireComponent] Freeze Freeze { get; set; }
    [RequireComponent] Teleport Teleport { get; set; }

	public static List<Player> GetAll() => Game.ActiveScene.GetAllComponents<Player>().ToList();
    public static Player GetOwnedBy(Connection connection) => GetAll().Find(p => p.Network.OwnerId == connection.Id);
    public static Player GetLocal() => GetOwnedBy(Connection.Local);
    public static Player GetHost() => GetOwnedBy(Connection.Host);
    public static Player GetRandom() => Game.Random.FromList(GetAll());

	public BaseRole CurrentRole => Role.Current;
	public void SetRole<T>() where T : BaseRole => Role.Set<T>();

	public float CurrentStamina => Stamina.Current;
	public float MaxStamina => Stamina.Max;

	public bool IsFrozen { get => Freeze.IsFrozen; set => Freeze.IsFrozen = value; }
	
	public void TeleportTo(Vector3 worldPosition) => Teleport.TeleportTo(worldPosition);
	
	[System.Obsolete]
    public void Seek() => Role.Set<SeekerRole>();
	[System.Obsolete]
    public void Hide() => Role.Set<HiderRole>();
	[System.Obsolete]
    public bool IsSeeker => Role.Current is SeekerRole;
	[System.Obsolete]
    public bool IsHider => Role.Current is HiderRole;
    
	public void Equip(Clothing clothing) => Clothes.Equip(clothing);

	public interface ISpawnListener : ISceneEvent<ISpawnListener>
	{
		void OnSpawned(Connection connection) { }
		void OnDespawned(Connection connection) { }
	}

	protected override void OnStart()
	{
		if (!Networking.IsHost) return;

		ISpawnListener.Post(e => e.OnSpawned(Network.Owner));
		Chat.SystemMessage($"{Network.Owner.DisplayName} joined the game");
	}

	protected override void OnDestroy()
	{
		if (!Networking.IsHost) return;

		ISpawnListener.Post(e => e.OnDespawned(Network.Owner));
		Chat.SystemMessage($"{Network.Owner.DisplayName} left the game");
	}
}
