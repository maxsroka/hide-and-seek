using Sandbox;

public sealed class PlayerRole : Component
{
	public void Hide() => CurrentRole = Role.Hider;
	public void Seek() => CurrentRole = Role.Seeker;

	[Sync( SyncFlags.FromHost ), Change( nameof( OnRoleChanged ) )]
	Role CurrentRole { get; set; } = Role.None;

	enum Role
	{
		None,
		Hider,
		Seeker
	}

	[Property]
	List<ClothingContainer.ClothingEntry> hiderClothing;
	[Property]
	List<ClothingContainer.ClothingEntry> seekerClothing;
	
	Dresser dresser;
	PlayerController playerController;

	protected override void OnStart()
	{
		dresser = GetComponent<Dresser>();
		playerController = GetComponent<PlayerController>();
		Hide();
	}

	void OnRoleChanged( Role oldRole, Role newRole )
	{
		Log.Error( "new role: " + newRole );

		if ( newRole == Role.Hider )
		{
			ChangeClothing( hiderClothing );
			playerController.ThirdPerson = true;
		}
		else if ( newRole == Role.Seeker )
		{
			ChangeClothing( seekerClothing );
		}
	}

	protected override void OnUpdate()
	{
		if (CurrentRole == Role.Seeker)
        {
			playerController.ThirdPerson = false;
        }
	}

	void ChangeClothing( List<ClothingContainer.ClothingEntry> clothing )
	{
		dresser.Clothing = clothing;
		dresser.Apply();
	}
	
	[ConCmd( "seek", ConVarFlags.Server )]
	static void Seek( Connection caller )
	{
		var player = Game.ActiveScene.FindAllWithTag( "player" ).First( p => p.Network.OwnerId == caller.Id );
		var playerRole = player.GetComponent<PlayerRole>();
		playerRole.Seek();
	}
	
	[ConCmd( "hide", ConVarFlags.Server )]
	static void Hide( Connection caller )
    {
		var player = Game.ActiveScene.FindAllWithTag( "player" ).First( p => p.Network.OwnerId == caller.Id );
		var playerRole = player.GetComponent<PlayerRole>();
		playerRole.Hide();
    }
}
