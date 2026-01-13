using Sandbox;
namespace HNS;

public class Playing : Stage
{
	[ConVar("play_time", ConVarFlags.GameSetting, Help = "Set the duration of a round.")]
	public static int PlayTime { get; set; } = 180;
	public override float Duration => PlayTime;

	public override void OnEnter()
	{
		Chat.SystemMessage("The Seeker has been released!");
	}

	public override void OnRun()
	{
		Round.Timer += Time.Delta;

		if (Round.Timer < PlayTime)
		{
			var players = Player.GetAll();

			if (players.Count == 0)
			{
				Round.Restart();
				return;
			}

			var hasSeekers = players.Any(p => p.CurrentRole is SeekerRole);
			var hasHiders = players.Any(p => p.CurrentRole is HiderRole);

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

	public override void OnJoin(Player player)
	{
		player.SetRole<SeekerRole>();
	}
}
