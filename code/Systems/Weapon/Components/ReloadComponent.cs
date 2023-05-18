using GameTemplate.Mechanics;
using Sandbox;

namespace GameTemplate.Weapons;

[Prefab]
public partial class Reload : WeaponComponent, ISingletonComponent
{

	//	[Net, Prefab] public float reloadTime { get; set; } = 4f;
	[Net] public bool isReloading { get; set; } = false;
	[Net, Prefab] public float reloadTime { get; set; } = 4f;
	protected override bool CanStart( Player player )
	{


		if ( !Input.Down( ("reload") ) ) return false;

		if ( isReloading ) return false;

		if ( Weapon.CurrentClip == Weapon.ClipSize ) return false; //if mag is full

		if ( player.GetAmmo( Weapon.AmmoType ) <= 0 ) return false;





		return true;

	}


	protected override void OnStart( Player player )
	{

		base.OnStart( player );

		player?.SetAnimParameter( "b_reload", true );

		isReloading = true;

		WeaponViewModel.Current?.SetAnimParameter( "reload", true );

		if ( Game.IsServer )
		{
			DoReloadEffects( To.Single( player ) );
		}
	}

	public override void Simulate( IClient cl, Player player )
	{
		base.Simulate( cl, player );

		if ( TimeSinceActivated >= reloadTime && isReloading )
		{
			ReloadGun( player );
		}

	}


	public async void CheckReload( Player player )
	{


	}

	[ClientRpc]
	public static void DoReloadEffects()
	{



	}


	protected virtual void ReloadGun( Player player )
	{

		var amount = player.TakeAmmo( Weapon.AmmoType, (Weapon.ClipSize - Weapon.CurrentClip) );
		Weapon.CurrentClip += amount;
		player.GetAmmo( Weapon.AmmoType );
		isReloading = false;
		RunGameEvent( "reload.finish" );

	}

	public void SafeReload( Player player )
	{
		if ( Game.IsServer )
		{
			OnStart( player );
		}

	}
}
