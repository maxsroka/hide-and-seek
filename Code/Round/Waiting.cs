using System;
using Sandbox;
namespace HNS;

public class Waiting : Stage
{
    [ConVar("min_players", ConVarFlags.GameSetting | ConVarFlags.Replicated, Help = "Set the minimum number of players required to begin.", Min = 1)]
    [Range(1, 20)]
    public static int MinPlayers { get; set; } = 2;

    [ConVar("wait_time", ConVarFlags.GameSetting | ConVarFlags.Replicated, Help = "Set the delay before a round starts.", Min = 0)]
    [Range(0, 30)]
    public static int WaitTime { get; set; } = 10;
	public override float Duration => WaitTime;

	int MissingPlayersCount => Math.Max(0, MinPlayers - Player.Count);
    bool IsStarting => MissingPlayersCount == 0;

	public override void OnRun()
    {
		base.OnRun();

		if (IsStarting)
		{
			Round.Timer += Time.Delta;

			if (Round.Timer >= WaitTime)
			{
				Round.Continue<Preparing>();
			}
		}
		else
		{
			Round.Timer = 0f;
		}
	}

	public override void OnJoin(Player player)
	{
		player.SetRole<HiderRole>();

        if (!IsStarting)
		{
			WaitingForPlayersMessage();
		}
	}

    void WaitingForPlayersMessage()
    {
        var players = MissingPlayersCount == 1 ? "player" : "players";
        Chat.SystemMessage($"Waiting for {MissingPlayersCount} more {players}...");
    }
}
