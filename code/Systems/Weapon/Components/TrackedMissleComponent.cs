/*
 
General idea - hl2 rocket launcher

spawn rocket (need rocket class???)

have line trace, make rocket move towards hit result (if it exists, else move towards last hit)


rocket:

has timer

AOE


*/

using GameTemplate.Mechanics;

namespace GameTemplate.Weapons;

[Prefab]
public partial class TrackedMissile : WeaponComponent, ISingletonComponent
{

	protected override bool CanStart( Player player )
	{
		if ( !Input.Down( ("attack1") ) ) return false;
		if ( Weapon.CurrentClip <= 0 ) return false;
		if ( Weapon.GetComponent<Reload>().isReloading ) return false;

		return true;
	}

	protected override void OnStart( Player player )
	{
		base.OnStart( player );

		player?.SetAnimParameter( "b_attack", true );

		// Send clientside effects to the player.
		RocketEntity model = new RocketEntity( "models/citizen/citizen.vmdl" );
		model.Position = new Vector3( 0, 0, 100 );


	}


}
