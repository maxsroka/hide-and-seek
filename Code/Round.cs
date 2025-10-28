using System;

public enum RoundStage
{
	Waiting,
	Preparing,
	Playing,
	Ending
}

enum RoundWinner
{
	Seekers,
	Hiders
}

public sealed class Round : Component
{
	/* Public Properties */

	public static Round Instance => Game.ActiveScene.Get<Round>();

	[Property, ReadOnly]
	[Sync(SyncFlags.FromHost)]
	public RoundStage Stage { get; private set; } = RoundStage.Waiting;

	/* Public Methods */

	public float GetTimeRemaining()
	{
		if (Stage == RoundStage.Waiting)
		{
			if (Player.GetAll().Count < MinPlayers)
			{
				return 0f;
			}

			return WaitTime - timer;
		}
		else if (Stage == RoundStage.Preparing)
		{
			return PrepTime - timer;
		}
		else if (Stage == RoundStage.Playing)
		{
			return PlayTime - timer;
		}
		else if (Stage == RoundStage.Ending)
		{
			return EndTime - timer;
		}

		return 0f;
	}

	/* Console Variables */

	[ConVar("min_players", ConVarFlags.GameSetting)]
	static int MinPlayers { get; set; } = 2;

	[ConVar("wait_time", ConVarFlags.GameSetting)]
	static float WaitTime { get; set; } = 30f;

	[ConVar("prep_time", ConVarFlags.GameSetting)]
	static float PrepTime { get; set; } = 5f;

	[ConVar("play_time", ConVarFlags.GameSetting)]
	static float PlayTime { get; set; } = 10f;

	[ConVar("end_time", ConVarFlags.GameSetting)]
	static float EndTime { get; set; } = 5f;

	[ConCmd("start", ConVarFlags.Server)]
	static void Start()
	{
		if (Instance.Stage != RoundStage.Waiting)
		{
			Log.Info("You can't start the game at this moment.");
			return;
		}

		Instance.Prepare();
	}
	
	/* Private Properties */

	[Property, ReadOnly]
	float timer = 0f;

	/* Events */

	protected override void OnUpdate()
	{
		TickTimer();
		CheckVictory();
	}
	
	/* Private Methods */

	void TickTimer()
	{
		if (!Connection.Local.IsHost) return;

		timer += Time.Delta;

		if (Stage == RoundStage.Waiting)
		{
			if (timer >= WaitTime)
			{
				Prepare();
			}

			if (Player.GetAll().Count < MinPlayers)
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
				End(RoundWinner.Hiders);
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

	void CheckVictory()
	{
		if (!Connection.Local.IsHost) return;
		if (Stage != RoundStage.Playing) return;

		var players = Player.GetAll();

		if (players.All(p => p.Role == Role.Seeker))
        {
			End(RoundWinner.Seekers);
        }
		else if (players.All(p => p.Role == Role.Hider))
		{
			End(RoundWinner.Hiders);
		}
	}

	void Wait()
	{
		Stage = RoundStage.Waiting;
		timer = 0f;
	}

	void Prepare()
	{
		Stage = RoundStage.Preparing;
		timer = 0f;

		var spawnPoint = GetRandomSpawnPoint();
		Player.GetAll().ForEach(player =>
		{
			player.Teleport(spawnPoint.WorldPosition);
			player.Role = Role.Hider;
		});

		var seeker = Player.GetRandom();
		seeker.Role = Role.Seeker;
		seeker.Freeze(true);
		seeker.Blind(true);

		Chat.Instance.Broadcast($"{seeker.Network.Owner.DisplayName} is the seeker!");
	}

	void Play()
	{
		Stage = RoundStage.Playing;
		timer = 0f;

		var seeker = Player.GetAll().Find(p => p.Role == Role.Seeker);
		seeker.Freeze(false);
		seeker.Blind(false);

		Chat.Instance.Broadcast("Ready or not, the hunt's begun!");
	}

	void End(RoundWinner winner)
	{
		Stage = RoundStage.Ending;
		timer = 0f;

		string message;
		if (winner == RoundWinner.Seekers)
		{
			message = "The seeking have won!";
		}
        else
        {
			message = "The hiding have won!";
        }
		Chat.Instance.Broadcast(message);
	}

	GameObject GetRandomSpawnPoint()
	{
		var spawnPoints = Game.ActiveScene.GetAllComponents<SpawnPoint>().ToList();
		var spawnPoint = Game.Random.FromList(spawnPoints);

		return spawnPoint.GameObject;
	}
}
