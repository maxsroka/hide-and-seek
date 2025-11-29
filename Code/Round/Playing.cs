using Sandbox;
namespace HNS;

public class Playing : Stage
{
	[ConVar("play_time", ConVarFlags.GameSetting)]
	public static int PlayTime { get; set; } = 180;
	public override float Duration => PlayTime;

	public override void OnEnter()
	{
		Chat.SystemMessage("The Seeker has been released!");
	}

	public override void OnRun()
	{
		Timer += Time.Delta;

		if (Timer < PlayTime)
		{
			var players = Player.GetAll();

			if (players.Count == 0)
			{
				Round.Restart();
				return;
			}

			var hasSeekers = players.Any(p => p.IsSeeker);
			var hasHiders = players.Any(p => p.IsHider);

			if (!hasSeekers || !hasHiders)
			{
				Round.Continue<Ending>();
			}
		}
		else
		{
			Round.Continue<Ending>();
		}
	}

	public override void OnPlayerJoined(Connection connection)
	{
		Player.GetOwnedBy(connection).SetRole<SeekerRole>();
	}
}
