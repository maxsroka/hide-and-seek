using Sandbox;

public sealed class PlayerRole : Component
{
	[Sync(SyncFlags.FromHost), Change(nameof(OnRoleChanged))]
	IRole Role { get; set; }

	HiderRole hiderRole;
	SeekerRole seekerRole;
	Dresser dresser;

	public void Hide() => Role = hiderRole;
	public void Seek() => Role = seekerRole;

	protected override void OnStart()
	{
		hiderRole = GetComponent<HiderRole>();
		seekerRole = GetComponent<SeekerRole>();
		dresser = GetComponent<Dresser>();
	}

	void OnRoleChanged(IRole oldRole, IRole newRole)
	{
		oldRole?.OnChanged( false, this);
		newRole.OnChanged( true, this);
		Log.Error( "new role: " + newRole.GetType().Name );
    }

	public interface IRole
	{
		public void OnChanged( bool assigned, PlayerRole playerRole );
	}

	public class HiderRole : Component, IRole
	{
		[Property]
		List<ClothingContainer.ClothingEntry> Clothing { get; set; }

        public void OnChanged( bool assigned , PlayerRole playerRole)
		{
			if ( assigned )
			{
				playerRole.ChangeClothing( Clothing );
			}
		}
    }

	public class SeekerRole : Component, IRole
	{
		[Property]
		List<ClothingContainer.ClothingEntry> Clothing { get; set; }

		public void OnChanged( bool assigned , PlayerRole playerRole)
		{
			if ( assigned )
			{
				playerRole.ChangeClothing( Clothing );
			}
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
		var player = Game.ActiveScene.FindAllWithTag( "Player" ).First( p => p.Network.OwnerId == caller.Id );
		var playerRole = player.GetComponent<PlayerRole>();
		playerRole.Seek();
	}
	
	[ConCmd( "hide", ConVarFlags.Server )]
	static void Hide( Connection caller )
    {
		var player = Game.ActiveScene.FindAllWithTag( "Player" ).First( p => p.Network.OwnerId == caller.Id );
		var playerRole = player.GetComponent<PlayerRole>();
		playerRole.Hide();
    }
}
