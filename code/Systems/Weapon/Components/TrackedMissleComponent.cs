/*
 
General idea - hl2 rocket launcher

spawn rocket (need rocket class???)

have line trace, make rocket move towards hit result pos (if it exists, else move towards last hit)


rocket:

has timer

AOE


*/

using GameTemplate.Mechanics;
using static Sandbox.Prefab;


namespace GameTemplate.Weapons;

[Prefab]
public partial class TrackedMissile : WeaponComponent, ISingletonComponent
{
	[Net] public Rocket RocketRef { get; set; }
	[Net] public Vector3 lastPos { get; set; }

	protected override bool CanStart( Player player )
	{

		if ( !Input.Down( ("attack1") ) ) return false;
		//if ( Weapon.CurrentClip <= 0 ) return false;
		//if ( Weapon.GetComponent<Reload>().isReloading ) return false;


		//if rocket exists
		if ( RocketRef != null ) return false;




		return true;
	}



	protected override void OnStart( Player player )
	{
		// works
		base.OnStart( player );

		player?.SetAnimParameter( "b_attack", true );

		// Send clientside effects to the player.
		if ( Game.IsServer )
		{
			DoShootEffects( To.Single( player ) );
		}

		if ( PrefabLibrary.TrySpawn<Rocket>( "prefabs/rocket.prefab", out var rocket ) )
		{

			RocketRef = rocket;
			RocketRef.Owner = player;
			RocketRef.Position = player.EyePosition + player.EyeRotation.Forward * 160;


		}

	}
	public override void Simulate( IClient cl, Player player )
	{
		base.Simulate( cl, player );

		TraceResult tr = Trace.Ray( player.AimRay, 200000000 ).WorldOnly().Run();
		if ( RocketRef != null )
		{

			lastPos = tr.EndPosition;
			if ( tr.Hit )
			{
				RocketRef.MoveTowards( tr.EndPosition );
			}
			else
			{
				RocketRef.MoveTowards( lastPos );
			}

		}

	}

	[ClientRpc]
	public static void DoShootEffects()
	{
		Game.AssertClient();
		WeaponViewModel.Current?.SetAnimParameter( "fire", true );
	}

}
