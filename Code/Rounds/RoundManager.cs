using Sandbox;

public sealed class RoundManager : Component
{
	[ConVar("prep_time")]
	static float PrepTime { get; set; } = 5f;

	[ConVar("play_time")]
	static float PlayTime { get; set; } = 10f;

	[ConVar("end_time")]
	static float EndTime { get; set; } = 5f;

	[ConCmd("start", ConVarFlags.Server)]
	static void Start()
	{
		var roundManager = Game.ActiveScene.Get<RoundManager>();
		roundManager.SetPreparing();
	}

	[Sync(SyncFlags.FromHost)]
	RoundStage Stage { get; set; } = RoundStage.Idle;

	float timer = 0f;

	protected override void OnUpdate()
	{
		if (!Connection.Local.IsHost) return;
		if (Stage == RoundStage.Idle) return;

		timer += Time.Delta;

		if (Stage == RoundStage.Preparing)
		{
			if (timer >= PrepTime)
			{
				SetPlaying();
				timer = 0f;
			}
		}
		else if (Stage == RoundStage.Playing)
		{
			if (timer >= PlayTime)
			{
				SetEnding();
				timer = 0f;
			}
		}
		else if (Stage == RoundStage.Ending)
		{
			if (timer >= EndTime)
			{
				SetIdle();
				timer = 0f;
			}
		}
	}

	void SetPreparing()
	{
		Stage = RoundStage.Preparing;
	}

	void SetPlaying()
	{
		Stage = RoundStage.Playing;
	}

	void SetEnding()
	{
		Stage = RoundStage.Ending;
	}

	void SetIdle()
	{
		Stage = RoundStage.Idle;
	}
}
