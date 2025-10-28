using System;

public enum RoundStage
{
    Waiting,
    Preparing,
    Playing,
    Ending
}

public sealed class Round : Component
{
	public static Round Instance => Game.ActiveScene.Get<Round>();

	[Property, ReadOnly]
	[Sync(SyncFlags.FromHost)]
	public RoundStage Stage { get; private set; } = RoundStage.Waiting;

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
	static void Start() => Instance.Prepare();

	[Property, ReadOnly]
	float timer = 0f;

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
	}

	void Prepare()
	{
		Stage = RoundStage.Preparing;
		timer = 0f;
		Chat.Instance.HostMessage("Preparing");

		var spawnPoint = GetRandomSpawnPoint();
		TeleportPlayersTo(spawnPoint.WorldPosition);

		var seeker = Player.GetRandom();
		seeker.Role = Role.Seeker;
		seeker.Freeze(true);
		seeker.Blind(true);
	}

	void Play()
	{
		Stage = RoundStage.Playing;
		timer = 0f;
		Chat.Instance.HostMessage("Playing");

		var seeker = Player.GetAll().Find(p => p.Role == Role.Seeker);
		seeker.Freeze(false);
		seeker.Blind(false);
	}

	void End()
	{
		Stage = RoundStage.Ending;
		timer = 0f;
		Chat.Instance.HostMessage("Ending");
	}

	GameObject GetRandomSpawnPoint()
	{
		var spawnPoints = Game.ActiveScene.GetAllComponents<SpawnPoint>().ToList();
		var spawnPoint = Game.Random.FromList(spawnPoints);

		return spawnPoint.GameObject;
	}

	void TeleportPlayersTo(Vector3 worldPosition)
    {
        foreach (var player in Player.GetAll())
		{
			player.Teleport(worldPosition);
        }
    }
}
