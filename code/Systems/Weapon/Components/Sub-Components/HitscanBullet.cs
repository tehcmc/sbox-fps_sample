using GameTemplate.Mechanics;
using Sandbox;

namespace GameTemplate.Weapons;

[Prefab]
public partial class HitscanBullet : ShootComponent
{

	[Net, Prefab] public float BaseDamage { get; set; }
	[Net, Prefab] public float BulletRange { get; set; }
	[Net, Prefab] public int BulletCount { get; set; }
	[Net, Prefab] public float BulletForce { get; set; }
	[Net, Prefab] public float BulletSize { get; set; }
	[Net, Prefab] public float BulletSpread { get; set; }
	[Net, Prefab] public float FireDelay { get; set; }

	public override void Simulate( IClient cl, Player player )
	{

	}
	public override void Tick( IClient cl, Player player )
	{

	}

	public override void Fire()
	{
		ShootBullet( BulletSpread, BulletForce, BulletSize, BulletCount, BulletRange );
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

	public void ShootBullet( float spread, float force, float bulletSize, int bulletCount = 1, float bulletRange = 5000f, float BaseDamage = 10f )
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

		}
	}
}

