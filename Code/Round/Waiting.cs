using System;
using Sandbox;
namespace HNS;

public class Waiting : Stage
{
    [ConVar("min_players", ConVarFlags.GameSetting)]
    [Change(nameof(OnMinPlayersChanged))]
    [Range(1, 20)]
    public static int MinPlayers { get; set; } = 2;

    [ConVar("wait_time", ConVarFlags.GameSetting)]
    [Range(0, 30)]
    public static int WaitTime { get; set; } = 10;
	public override float Duration => WaitTime;

	int MissingPlayersCount => Math.Max(0, MinPlayers - Connection.All.Count);
    bool IsStarting => MissingPlayersCount == 0;
    
    static void OnMinPlayersChanged(int oldValue, int newValue)
    {
        if (!Game.IsPlaying) return;
        if (!Networking.IsHost) return;

        var instance = Game.ActiveScene.Get<Waiting>();
        if (instance == null) return;
        if (instance.IsStarting) return;

        instance.WaitingForPlayersMessage();
    }

	public override void OnPlayerJoined(Connection connection)
	{
		Player.GetOwnedBy(connection).SetRole<HiderRole>();

        if (IsStarting) return;

        WaitingForPlayersMessage();
	}

    public override void OnRun()
    {
        if (IsStarting)
        {
			Timer += Time.Delta;

            if (Timer >= WaitTime)
            {
                Round.Continue<Preparing>();
            }
        }
        else
        {
			Timer = 0f;
        }
    }

    void WaitingForPlayersMessage()
    {
        var players = MissingPlayersCount == 1 ? "player" : "players";
        Chat.SystemMessage($"Waiting for {MissingPlayersCount} more {players}...");
    }
}
