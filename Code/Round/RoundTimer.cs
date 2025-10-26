using System;
using Sandbox;

public sealed class RoundTimer : Component
{
	[Property, ReadOnly]
	[Sync(SyncFlags.FromHost)]
	public RoundStage Stage { get; private set; } = RoundStage.Waiting;
	
	[ConVar("prep_time")]
	static float PrepTime { get; set; } = 5f;

	[ConVar("play_time")]
	static float PlayTime { get; set; } = 10f;

	[ConVar("end_time")]
	static float EndTime { get; set; } = 5f;

	[Property, ReadOnly]
	float timer = 0f;

	protected override void OnUpdate()
	{
		if (!Connection.Local.IsHost) return;
		if (Stage == RoundStage.Waiting) return;

		timer += Time.Delta;

		if (Stage == RoundStage.Preparing)
		{
			if (timer >= PrepTime)
			{
				Stage = RoundStage.Playing;
				timer = 0f;
				IRoundEvent.Post(e => e.OnPlaying());
			}
		}
		else if (Stage == RoundStage.Playing)
		{
			if (timer >= PlayTime)
			{
				Stage = RoundStage.Ending;
				timer = 0f;
				IRoundEvent.Post(e => e.OnEnding());
			}
		}
		else if (Stage == RoundStage.Ending)
		{
			if (timer >= EndTime)
			{
				Stage = RoundStage.Waiting;
				timer = 0f;
				IRoundEvent.Post(e => e.OnWaiting());
			}
		}
	}

	[ConCmd("start", ConVarFlags.Server)]
	static void Start()
	{
        var roundTimer = Game.ActiveScene.Get<RoundTimer>();
		roundTimer.Stage = RoundStage.Preparing;
		roundTimer.timer = 0f;
		IRoundEvent.Post(e => e.OnPreparing());
	}
}
