using System;
using Sandbox;
using Sandbox.Diagnostics;

public sealed class PlayerRole : Component, Component.ITriggerListener
{
	public void Hide() => CurrentRole = Role.Hider;
	public void Seek() => CurrentRole = Role.Seeker;

	[Sync( SyncFlags.FromHost ), Change( nameof( OnRoleChanged ) )]
	Role CurrentRole { get; set; } = Role.None;
	float stamina = 5f;

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

	[Property]
	CapsuleCollider standingCollider;
	[Property]
	CapsuleCollider crouchingCollider;
	
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
		HideAndSeekLogger.Info( $"Player '{Network.Owner.DisplayName}' is now a {newRole.ToString()}" );

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
		if ( Network.IsOwner )
		{
			if ( CurrentRole == Role.Seeker )
			{
				playerController.ThirdPerson = false;
			}
		}

		if ( !playerController.IsDucking )
		{
			standingCollider.Enabled = true;
			crouchingCollider.Enabled = false;
		}
		else
		{
			standingCollider.Enabled = false;
			crouchingCollider.Enabled = true;
		}

		if ( Input.Down( "run" ) )
		{
			stamina = MathF.Max( 0, stamina - Time.Delta );
		}
		else
		{
			stamina = Math.Min( 5f, stamina + Time.Delta );
		}

		HideAndSeekLogger.Error( stamina );

		if ( stamina > 0f )
		{
			playerController.AltMoveButton = "run";
		}
		else
		{

			playerController.AltMoveButton = "";
		}

		if ( CurrentRole == Role.Seeker)
		{
			playerController.RunSpeed = 400;
		}
		else if (CurrentRole == Role.Hider)
        {
			playerController.RunSpeed = 350;
        }
	}
	
	public void OnTriggerEnter( Collider other )
	{
		if (Connection.Local.IsHost)
		{
			if (CurrentRole == Role.Seeker)
			{			
				if (other.Tags.Contains("player"))
				{
					var playerRole = other.GetComponent<PlayerRole>();

					if (playerRole != null)
                    {
						if(playerRole.CurrentRole == Role.Hider)
						{
							playerRole.Seek();
						}
                    }
				}
			}
        }
    }

	public void OnTriggerExit( Collider other )
    {
        
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
