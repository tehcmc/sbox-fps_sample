using GameTemplate.Mechanics;
using Sandbox;

namespace GameTemplate.Weapons;

[Prefab]
public partial class PrimaryFire : WeaponComponent, ISingletonComponent
{
	[Net, Prefab] public float BaseDamage { get; set; }
	[Net, Prefab] public float BulletRange { get; set; }
	[Net, Prefab] public int BulletCount { get; set; }
	[Net, Prefab] public float BulletForce { get; set; }
	[Net, Prefab] public float BulletSize { get; set; }
	[Net, Prefab] public float BulletSpread { get; set; }
	[Net, Prefab] public float FireDelay { get; set; }

	[Net] public Rocket RocketRef { get; set; }
	[Net] public Vector3 lastPos { get; set; }



	[Net, Prefab, ResourceType( "sound" )] public string FireSound { get; set; }


	[Net] public bool isFiring { get; set; } = false;

	TimeUntil TimeUntilCanFire { get; set; }
	public override void Initialize( Weapon weapon )
	{
		base.Initialize( weapon );
		Components.Add( PrimaryProjectile );


	}
	protected override bool CanStart( Player player )
	{

		if ( !Input.Down( ("attack1") ) ) return false;
		if ( TimeUntilCanFire > 0 ) return false;
		if ( Weapon.CurrentClip <= 0 ) return false;
		if ( Weapon.GetComponent<Reload>().isReloading ) return false;

		return TimeSinceActivated > FireDelay;
	}

	public override void OnGameEvent( string eventName )
	{
		if ( eventName == "sprint.stop" )
		{
			TimeUntilCanFire = 1.2f;
		}
	}

	protected override void OnStart( Player player )
	{
		base.OnStart( player );
		isFiring = true;
		player?.SetAnimParameter( "b_attack", true );

		// Send clientside effects to the player.
		if ( Game.IsServer )
		{
			player.PlaySound( FireSound );
			DoShootEffects( To.Single( player ) );
		}


		ShootBullet( BulletSpread, BulletForce, BulletSize, BulletCount, BulletRange );

	}


	[ClientRpc]
	public static void DoShootEffects()
	{
		Game.AssertClient();
		WeaponViewModel.Current?.SetAnimParameter( "fire", true );
	}

	public IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius )
	{
		var tr = Trace.Ray( start, end )
			.UseHitboxes()
			.WithAnyTags( "solid", "player", "glass" )
			.Ignore( Entity )
			.Size( radius )
			.Run();

		if ( tr.Hit )
		{
			yield return tr;
		}
	}

	public void ShootBullet( float spread, float force, float bulletSize, int bulletCount = 1, float bulletRange = 5000f )
	{
		//
		// Seed rand using the tick, so bullet cones match on client and server
		//
		Game.SetRandomSeed( Time.Tick );




		for ( int i = 0; i < bulletCount; i++ )
		{

			var rot = Rotation.LookAt( Player.AimRay.Forward );

			var forward = rot.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			Weapon.CurrentClip--;

			var damage = BaseDamage;

			foreach ( var tr in TraceBullet( Player.AimRay.Position, Player.AimRay.Position + forward * bulletRange, bulletSize ) )
			{
				tr.Surface.DoBulletImpact( tr );

				if ( !Game.IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;

				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Player )
					.WithWeapon( Weapon );

				tr.Entity.TakeDamage( damageInfo );

			}
			isFiring = false;
		}
	}
}
