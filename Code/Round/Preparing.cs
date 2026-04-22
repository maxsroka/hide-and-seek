using Sandbox;
namespace HNS;

public class Preparing : Stage
{
	[ConVar("prep_time", ConVarFlags.GameSetting | ConVarFlags.Replicated, Help = "Set the preparation time before a round begins.", Min = 0)]
	[Range(0, 30)]
	public static int PrepTime { get; set; } = 10;
	public override float Duration => PrepTime;

	Player seeker = null;

	public override void OnEnter()
	{
		var players = Player.GetAll();
		
		seeker = Game.Random.FromList(players);
		seeker.SetRole<SeekerRole>();
		seeker.IsFrozen = true;
		Chat.SystemMessage($"{seeker.Network.Owner.DisplayName} is the Seeker!");

		var spawnPoint = GetRandomSpawnPoint();
		players.ForEach(p => p.Teleport(spawnPoint.WorldPosition));
	}

	public override void OnRun()
	{
		base.OnRun();

		Round.Timer += Time.Delta;

		if (Round.Timer >= PrepTime)
		{
			Round.Continue<Playing>();
		}
	}

	public override void OnExit()
	{
		if (!seeker.IsValid) return;

		seeker.IsFrozen = false;
	}

	public override void OnJoin(Player player)
	{
		player.SetRole<HiderRole>();
	}

	GameObject GetRandomSpawnPoint()
	{
		var spawnPoints = Game.ActiveScene.GetAllComponents<SpawnPoint>().ToList();
		var spawnPoint = Game.Random.FromList(spawnPoints);

		return spawnPoint.GameObject;
	}

	public override string GetDescription()
	{
		var localPlayer = Player.GetLocal();
		if (localPlayer == null) return "Loading...";

		return localPlayer.CurrentRole is HiderRole ? "Time to hide!" : "You're seeking!";
	}
}
