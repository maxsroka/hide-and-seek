using Sandbox;
namespace HNS;

public class Playing : Stage
{
	[ConVar("play_time", ConVarFlags.GameSetting)]
	public static int PlayTime { get; set; } = 10;

	float timer = 0f;

	public override void OnEnter()
	{
		Chat.SystemMessage("The Seeker has been released!");
	}

	public override void OnRun()
	{
		timer += Time.Delta;

		if (timer < PlayTime)
		{
			var players = Player.GetAll();

			if (players.Count == 0)
			{
				Round.Stop();
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
}
