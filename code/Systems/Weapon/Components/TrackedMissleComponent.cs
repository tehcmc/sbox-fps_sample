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
	[Net, Prefab, ResourceType( "sound" )] public string FireSound { get; set; }

	[Net, Prefab, ResourceType( "prefab" )] public string RocketToFire { get; set; }
	protected override bool CanStart( Player player )
	{
		if ( !Input.Down( ("attack1") ) ) return false;
		if ( Weapon.CurrentClip <= 0 ) return false;
		if ( Weapon.GetComponent<Reload>().isReloading ) return false;

		if ( RocketRef != null ) return false;  //if rocket exists




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

			player.PlaySound( FireSound );
			DoShootEffects( To.Single( player ) );
			ShootRocket( player );

		}
	}
	public virtual void ShootRocket( Player player )
	{
		Weapon.CurrentClip--;
		if ( Game.IsServer )
		{
			if ( PrefabLibrary.TrySpawn<Rocket>( RocketToFire, out var rocket ) )
			{

				RocketRef = rocket;
				RocketRef.Owner = player;
				RocketRef.Position = player.EyePosition + player.EyeRotation.Forward * 100;

			}
			if ( Weapon.CurrentClip <= 0 )
			{

			}
		}
	}
	public override void Simulate( IClient cl, Player player )
	{
		base.Simulate( cl, player );


		if ( RocketRef != null )
		{
			if ( RocketRef.IsBeingDestroyed() )
			{
				Weapon.GetComponent<Reload>().SafeReload( player );
			}
		}

	}
	public override void Tick( IClient cl, Player player )
	{
		base.Tick( cl, player );

		if ( RocketRef != null )
		{
			TraceResult tr = Trace.Ray( player.AimRay, 200000000 ).WorldAndEntities().Ignore( player ).Ignore( RocketRef ).Run();
			lastPos = tr.EndPosition;
			if ( tr.Hit )
			{
				RocketRef.TargetPos = Vector3.Lerp( lastPos, tr.EndPosition, 0.5f );
			}
			else
			{

				RocketRef.TargetPos = lastPos;
			}

			RocketRef.Simulate( cl );

			if ( RocketRef != null )
			{
				if ( RocketRef.IsBeingDestroyed() )
				{
					Weapon.GetComponent<Reload>().SafeReload( player );
				}
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
