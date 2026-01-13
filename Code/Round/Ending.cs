using Sandbox;
using Sandbox.Services;
namespace HNS;

public class Ending : Stage
{
	[ConVar("end_time", ConVarFlags.GameSetting, Help = "Set the delay before a round ends.")]
	[Range(0, 30)]
	public static int EndTime { get; set; } = 5;
	public override float Duration => EndTime;

	public override void OnEnter()
	{
		var players = Player.GetAll();

		if (players.Count == 0)
		{
			Round.Restart();
			return;
		}

		var hasHiders = players.Any(p => p.CurrentRole is HiderRole);

		if (hasHiders)
		{
			Chat.SystemMessage("Time's up. The Hiders win!");
		}
		else
		{
			Chat.SystemMessage("Everyone's been caught. The Seekers win!");
		}

		PlaySound();
		RecordStats(hasHiders);
	}

	public override void OnRun()
	{
		Round.Timer += Time.Delta;

		if (Round.Timer >= EndTime)
		{
			Round.Restart();
		}
	}

	public override void OnJoin(Player player)
	{
		player.SetRole<SeekerRole>();
	}

	[Rpc.Broadcast(NetFlags.HostOnly)]
	void PlaySound()
	{
		Sound.Play("ending");
	}

	[Rpc.Broadcast(NetFlags.HostOnly)]
	void RecordStats(bool hidersWon)
	{
		Stats.Increment("round-ends", 1);
		
		if (hidersWon)
		{
			Stats.Increment("hider-wins", 1);
		}
		else
		{
			Stats.Increment("seeker-wins", 1);
		}
	}
}
