using Sandbox;
namespace Round;

public class Preparing : Stage
{
	[ConVar("prep_time", ConVarFlags.GameSetting)]
	[Range(0, 30)]
	public static int PrepTime { get; set; } = 10;

	float timer = 0f;
	Player seeker = null;

	public override void OnEnter()
	{
		var players = Player.GetAll();
		
		var spawnPoint = GetRandomSpawnPoint();
		foreach (var player in players)
		{
			player.Teleport(spawnPoint.WorldPosition);
		}

		seeker = Game.Random.FromList(players);
		seeker.Seek();
		seeker.Freeze(true);
		Chat.Instance.SystemMessage($"{seeker.Network.Owner.DisplayName} is the Seeker!");
	}

	public override void OnRun()
	{
		timer += Time.Delta;

		if (timer >= PrepTime)
		{
			Round.Continue<Playing>();
		}
	}

	public override void OnExit()
	{
		seeker?.Freeze(false);
	}

	GameObject GetRandomSpawnPoint()
	{
		var spawnPoints = Game.ActiveScene.GetAllComponents<SpawnPoint>().ToList();
		var spawnPoint = Game.Random.FromList(spawnPoints);

		return spawnPoint.GameObject;
	}
}
