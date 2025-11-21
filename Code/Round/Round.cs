using Sandbox;
using Sandbox.Diagnostics;
namespace HNS;

public class Round : Component, Component.INetworkListener, Player.ISpawnListener
{
    [Sync(SyncFlags.FromHost)]
    [Change(nameof(OnStageChanged))]
    Stage Stage { get; set; } = null;

    protected override void OnStart()
    {
        if (!Networking.IsHost) return;
        
        Continue<Waiting>();
    }

    void OnStageChanged(Stage oldStage, Stage newStage)
    {
        if (!Networking.IsHost) return;

        oldStage?.OnExit();
        newStage.OnEnter();
    }

    protected override void OnUpdate()
    {
        if (!Networking.IsHost) return;
        if (Stage == null) return;

        Stage.OnRun();
    }
    
    void Player.ISpawnListener.OnSpawned(Connection connection)
    {
        Stage?.OnPlayerJoined(connection);
	}

    void INetworkListener.OnDisconnected(Connection connection)
    {
        Stage?.OnPlayerLeft(connection);
    }

    public void Continue<T>() where T : Stage
    {
        Assert.True(Networking.IsHost);
        Stage = GetComponent<T>(true);
    }

	public void Stop()
	{
		Assert.True(Networking.IsHost);

		var options = new SceneLoadOptions();
		options.SetScene("hideandseek.scene");
		Game.ChangeScene(options);
	}
}
