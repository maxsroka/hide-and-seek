using Sandbox;
using Sandbox.Diagnostics;
namespace HNS;

public class Round : Component, Component.INetworkListener, Player.ISpawnListener
{
    [Sync(SyncFlags.FromHost)]
    [Change(nameof(OnStageChanged))]
    Stage Stage { get; set; } = null;
	
	[Sync(SyncFlags.FromHost)]
	public float Timer { get; set; } = 0f;

	public float TimeLeft => Stage != null ? Stage.Duration - Timer : 0f;

	public string GetDescription()
	{
		return Stage?.GetDescription();
	}

    protected override void OnStart()
    {
        if (!Networking.IsHost) return;
        
        Continue<Waiting>();
    }

    void OnStageChanged(Stage oldStage, Stage newStage)
    {
        if (!Networking.IsHost) return;

        oldStage?.OnExit();
		Timer = 0f;
        newStage.OnEnter();
    }

    protected override void OnUpdate()
    {
        if (!Networking.IsHost) return;
        if (Stage == null) return;

        Stage.OnRun();
	}
    
    void Player.ISpawnListener.OnSpawned(Player player)
    {
		Chat.SystemMessage($"{player.Network.Owner.DisplayName} joined the game");
        Stage?.OnJoin(player);
	}

    void INetworkListener.OnDisconnected(Connection connection)
    {
		Chat.SystemMessage($"{connection.DisplayName} left the game");
	}

    public void Continue<T>() where T : Stage
    {
        Assert.True(Networking.IsHost);
        Stage = GetComponent<T>(true);
	}

	public void Restart()
	{
		Assert.True(Networking.IsHost);
		var options = new SceneLoadOptions();
		options.SetScene("hideandseek.scene");
		Game.ChangeScene(options);
	}

	[Rpc.Broadcast(NetFlags.Unreliable | NetFlags.HostOnly)]
	public void PlayTimerSound()
	{
		Sound.Play("timer");
	}
}
