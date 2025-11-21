using Sandbox;
namespace HNS;

public class Ending : Stage
{
	[ConVar("end_time", ConVarFlags.GameSetting)]
	[Range(0, 30)]
	public static int EndTime { get; set; } = 5;

	float timer = 0f;

	public override void OnEnter()
	{
		var players = Player.GetAll();

		if (players.Count == 0)
		{
			Round.Stop();
			return;
		}

		var hasHiders = players.Any(p => p.IsHider);

		if (hasHiders)
		{
			Chat.SystemMessage("Time's up. The Hiders win!");
		}
		else
		{
			Chat.SystemMessage("Everyone's been caught. The Seekers win!");
		}
	}

	public override void OnRun()
	{
		timer += Time.Delta;

		if (timer >= EndTime)
		{
			Round.Stop();
		}
	}

	public override void OnPlayerJoined(Connection connection)
	{
		Player.GetOwnedBy(connection).Seek();
	}
}
