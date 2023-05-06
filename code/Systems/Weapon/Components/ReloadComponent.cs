using GameTemplate.Mechanics;

namespace GameTemplate.Weapons;

[Prefab]
public partial class Reload : WeaponComponent, ISingletonComponent
{
	[Net, Prefab] public int ReloadAmmo { get; set; } = 7;
	protected override bool CanStart( Player player )
	{

		if ( !Input.Down( ("reload") ) ) return false;

		if ( Weapon.CurrentClip == Weapon.ClipSize ) return false; //if mag is full


		if ( Weapon.AmmoReserve <= 0 ) return false;


		return true;
	}


	protected override void OnStart( Player player )
	{
		base.OnStart( player );
		ReloadGun( player );

	}

	protected virtual void ReloadGun( Player player )
	{

		//for now just set clip back to max size
		//Weapon.CurrentClip = Weapon.ClipSize;

		//todo - create table of ammotypes on player character, get ammo count and subtract from that to reload. Possibly for loop so that ammo below mag size can reload safely.


		var amount = player.TakeAmmo( Weapon.AmmoType, (Weapon.ClipSize - Weapon.CurrentClip) );
		Weapon.CurrentClip += amount;
		player.GetAmmo( Weapon.AmmoType );

		/*
		for ( int i = 0; i < Weapon.ClipSize; i++ )
		{
			if ( Weapon.CurrentClip >= Weapon.ClipSize || Weapon.AmmoReserve <= 0 )
			{
				break;
			}
			else
			{
				Weapon.AmmoReserve--;
				Weapon.CurrentClip++;
			}

		}
		*/
	}
}
