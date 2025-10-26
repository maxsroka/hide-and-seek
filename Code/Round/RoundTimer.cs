using System;
using Sandbox;

public sealed class RoundTimer : Component
{
	[Property, ReadOnly]
	[Sync(SyncFlags.FromHost)]
	public RoundStage Stage { get; private set; } = RoundStage.Waiting;
	
	[ConVar("min_players")]
	static int MinPlayers { get; set; } = 2;

	[ConVar("wait_time")]
	static float WaitTime { get; set; } = 30f;

	[ConVar("prep_time")]
	static float PrepTime { get; set; } = 5f;

	[ConVar("play_time")]
	static float PlayTime { get; set; } = 10f;

	[ConVar("end_time")]
	static float EndTime { get; set; } = 5f;

	[Property, ReadOnly]
	float timer = 0f;

	protected override void OnStart()
	{
		base.OnStart();
	}

	protected override void OnUpdate()
	{
		if (!Connection.Local.IsHost) return;

		timer += Time.Delta;

		if (Stage == RoundStage.Waiting)
        {
			if (timer >= WaitTime)
			{
				Prepare();
			}

			if (PlayerFinder.All.Count() < MinPlayers)
			{
				timer = 0f;
            }
        }
		else if (Stage == RoundStage.Preparing)
		{
			if (timer >= PrepTime)
			{
				Play();
			}
		}
		else if (Stage == RoundStage.Playing)
		{
			if (timer >= PlayTime)
			{
				End();
			}
		}
		else if (Stage == RoundStage.Ending)
		{
			if (timer >= EndTime)
			{
				Prepare();
			}
		}
	}

	void Wait()
    {
		Stage = RoundStage.Waiting;
		timer = 0f;
		IRoundEvent.Post(e => e.OnWaiting());
    }

	void Prepare()
	{
		Stage = RoundStage.Preparing;
		timer = 0f;
		IRoundEvent.Post(e => e.OnPreparing());
	}

	void Play()
    {
		Stage = RoundStage.Playing;
		timer = 0f;
		IRoundEvent.Post(e => e.OnPlaying());
    }
	
	void End()
    {
		Stage = RoundStage.Ending;
		timer = 0f;
		IRoundEvent.Post(e => e.OnEnding());
    }

	[ConCmd("start", ConVarFlags.Server)]
	static void Start()
	{
		var roundTimer = Game.ActiveScene.Get<RoundTimer>();
		roundTimer.Prepare();
	}
}
