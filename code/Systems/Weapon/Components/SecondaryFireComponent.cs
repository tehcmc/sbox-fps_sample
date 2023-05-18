using GameTemplate.Mechanics;
using Sandbox;

namespace GameTemplate.Weapons;

[Prefab]
public partial class SecondaryFire : WeaponComponent, ISingletonComponent
{
	[Net, Prefab] BulletType bulletType = BulletType.Hitscan;

	protected override bool CanStart( Player player )
	{
		if ( !Input.Down( "secondary" ) ) return false;

		return true;
	}

	protected override void OnStart( Player player )
	{
		base.OnStart( player );

		switch ( bulletType )
		{
			case BulletType.Hitscan: //hitscan
				break;

			case BulletType.Projectile:
				break;


				break;
		}

	}

}

public virtual void HitscanFire()
{

}

public virtual void ProjectileFire()
{

}



public enum BulletType
{
	Hitscan,
	Projectile
}
