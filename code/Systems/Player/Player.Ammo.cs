using GameTemplate.Weapons;
using Sandbox;
using System.Collections;

namespace GameTemplate;

public partial class Player
{
	//[Net, Prefab] public int StarterAmmo { get; set; } = 100;


	[Net] public IDictionary<WeaponAmmoType, int> AmmoTable { get; set; } = new Dictionary<WeaponAmmoType, int>();
	public virtual void SetUpAmmo()
	{
		AmmoTable.Add( WeaponAmmoType.Pistol, 100 );
		AmmoTable.Add( WeaponAmmoType.SMG, 200 );
		AmmoTable.Add( WeaponAmmoType.Rifle, 200 );
	}

	public virtual int GetAmmo( WeaponAmmoType key )
	{
		if ( AmmoTable.ContainsKey( key ) == false ) return 0;

		Vector2 pos = new( 100, 150 );

		return (int)AmmoTable[key];
	}

	public int TakeAmmo( WeaponAmmoType key, int amount )
	{
		if ( (AmmoTable == null) ) return 0;

		var available = AmmoTable[key];

		amount = Math.Min( available, amount );



		SetAmmo( key, available - amount );
		return amount;
	}

	public void SetAmmo( WeaponAmmoType key, int amount )
	{
		AmmoTable[key] = amount;
	}

	public void DestroyAmmoTable()
	{
		AmmoTable.Clear();
	}
}
