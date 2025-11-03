using System;
using Sandbox;
namespace Round;

// so, this is pretty cool but 
// x 1. it needs to run only on host, 
// x 2. it needs to run only when it is the current stage, 
// 3. convars shouldn't be changed by clients,
// x 4. what should happen when a client leaves?
// 5. is there a possibility to start before a player object is spawned?

public class Waiting : Stage
{
    [ConVar("min_players", ConVarFlags.GameSetting)]
    [Change(nameof(OnMinPlayersChanged))]
    [Range(1, 20)]
    public static int MinPlayers { get; set; } = 2;

    [ConVar("wait_time", ConVarFlags.GameSetting)]
    [Range(0, 30)]
    public static int WaitTime { get; set; } = 10;

    int MissingPlayersCount => Math.Max(0, MinPlayers - Connection.All.Count);
    bool IsStarting => MissingPlayersCount == 0;
    
    float timer = 0f;

    static void OnMinPlayersChanged(int oldValue, int newValue)
    {
        if (!Game.IsPlaying) return;
        if (!Networking.IsHost) return;

        var instance = Game.ActiveScene.Get<Waiting>();
        if (instance == null) return;
        if (instance.IsStarting) return;

        instance.WaitingForPlayersMessage();
    }

    public override void OnEnter()
    {
        if (IsStarting) return;

        WaitingForPlayersMessage();
    }

	public override void OnPlayerJoined(Connection connection)
	{
        if (IsStarting) return;

        WaitingForPlayersMessage();
	}

    public override void OnRun()
    {
        if (IsStarting)
        {
            timer += Time.Delta;

            if (timer >= WaitTime)
            {
                Round.Continue<Preparing>();
            }
        }
        else
        {
            timer = 0f;
        }
    }

    void WaitingForPlayersMessage()
    {
        var players = MissingPlayersCount == 1 ? "player" : "players";
        Chat.Instance.SystemMessage($"Waiting for {MissingPlayersCount} more {players}...");
    }
}