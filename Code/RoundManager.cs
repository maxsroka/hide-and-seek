using Sandbox;

public sealed class RoundManager : Component
{
	enum RoundStage
	{
		Idle,
		Preparing,
		Gameplay,
		Ending
	}

	RoundStage Stage { get; set; } = RoundStage.Idle;
	float timer;
	float preparingTime = 5f;
	float gameplayTime = 10f;
	float endingTime = 5f;

	protected override void OnUpdate()
	{
		if ( Stage == RoundStage.Idle ) return;

		timer += Time.Delta;

		if ( Stage == RoundStage.Preparing )
		{
			if ( timer >= preparingTime )
			{
				SetGameplay();
				timer = 0f;
			}
		}
		else if ( Stage == RoundStage.Gameplay )
		{
			if ( timer >= gameplayTime )
			{
				SetEnding();
				timer = 0f;
			}
		}
		else if ( Stage == RoundStage.Ending )
		{
			if ( timer >= endingTime )
			{
				SetIdle();
				timer = 0f;
			}
		}

		HideAndSeekLogger.Error( Stage.ToString() + ": " + timer );
	}

	void SetPreparing()
	{
		Stage = RoundStage.Preparing;
	}

	void SetGameplay()
	{
		Stage = RoundStage.Gameplay;
	}

	void SetEnding()
	{
		Stage = RoundStage.Ending;
	}
	
	void SetIdle()
    {
		Stage = RoundStage.Idle;
    }
	
	[ConCmd( "start", ConVarFlags.Server )]
	static void Start()
	{
		var roundManager = Game.ActiveScene.GetAllComponents<RoundManager>().First();
		roundManager.SetPreparing();
	}
}
