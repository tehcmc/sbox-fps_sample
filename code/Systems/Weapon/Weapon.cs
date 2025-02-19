namespace GameTemplate.Weapons;

[Prefab, Title( "Weapon" ), Icon( "track_changes" )]
public partial class Weapon : AnimatedEntity
{
	// Won't be Net eventually, when we serialize prefabs on client
	[Net, Prefab, Category( "Animation" )] public WeaponHoldType HoldType { get; set; } = WeaponHoldType.Pistol;

	[Net, Prefab, Category( "Ammunition" )] public WeaponAmmoType AmmoType { get; set; } = WeaponAmmoType.Pistol;

	[Net, Prefab, Category( "Ammunition" )] public int AmmoReserve { get; set; } = 300;
	[Net, Prefab, Category( "Animation" )] public WeaponHandedness Handedness { get; set; } = WeaponHandedness.Both;

	[Net, Prefab] public FireType SelectedType { get; set; } = FireType.Automatic;
	[Net, Prefab, Category( "Animation" )] public float HoldTypePose { get; set; } = 0;

	[Net, Prefab] public bool DoTick { get; set; } = false;
	[Net, Prefab] public int ClipSize { get; set; }
	[Net] public int CurrentClip { get; set; }



	public AnimatedEntity EffectEntity => ViewModelEntity.IsValid() ? ViewModelEntity : this;
	public WeaponViewModel ViewModelEntity { get; protected set; }
	public Player Player => Owner as Player;

	public override void Spawn()
	{


		CurrentClip = ClipSize;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableDrawing = false;

	}

	/// <summary>
	/// Can we holster the weapon right now? Reasons to reject this could be that we're reloading the weapon..
	/// </summary>
	/// <returns></returns>
	public bool CanHolster( Player player )
	{
		return true;
	}

	/// <summary>
	/// Called when the weapon gets holstered.
	/// </summary>
	public void OnHolster( Player player )
	{
		EnableDrawing = false;

		if ( Game.IsServer )
			DestroyViewModel( To.Single( player ) );
	}

	/// <summary>
	/// Can we deploy this weapon? Reasons to reject this could be that we're performing an action.
	/// </summary>
	/// <returns></returns>
	public bool CanDeploy( Player player )
	{
		return true;
	}

	/// <summary>
	/// Called when the weapon gets deployed.
	/// </summary>
	public void OnDeploy( Player player )
	{
		SetParent( player, true );
		Owner = player;

		EnableDrawing = true;

		if ( Game.IsServer )
			CreateViewModel( To.Single( player ) );
	}

	[ClientRpc]
	public void CreateViewModel()
	{
		if ( GetComponent<ViewModelComponent>() is not ViewModelComponent comp ) return;

		var vm = new WeaponViewModel( this );
		vm.Model = Model.Load( comp.ViewModelPath );
		ViewModelEntity = vm;
	}

	[ClientRpc]
	public void DestroyViewModel()
	{
		if ( ViewModelEntity.IsValid() )
		{
			ViewModelEntity.Delete();
		}
	}

	public override void Simulate( IClient cl )
	{
		SimulateComponents( cl );
	}

	public virtual void Tick( IClient cl )
	{
		if ( DoTick )
		{
			TickComponents( cl );
		}
	}


	protected override void OnDestroy()
	{
		ViewModelEntity?.Delete();
	}

	public override string ToString()
	{
		return $"Weapon ({Name})";
	}
}

/// <summary>
/// Describes the holdtype of a weapon, which tells our animgraph which animations to use.
/// </summary>
public enum WeaponHoldType
{
	None,
	Pistol,
	Rifle,
	Shotgun,
	Item,
	Fists,
	Swing
}


/// <summary>
/// Describes the handedness of a weapon, which hand (or both) we hold the weapon in.
/// </summary>
public enum WeaponHandedness
{
	Both,
	Right,
	Left
}
/// <summary>
/// Fire mode of gun. Should be able to be toggled, along with enabling/disabling certain types (ie single-fire only gun)
/// </summary>
public enum FireType
{
	Safe,
	Single,
	Automatic,
	Burst
}

/// <summary>
/// Ammo type of a weapon. Get ammo count from player.
/// </summary>
public enum WeaponAmmoType
{
	None,
	Pistol,
	SMG,
	Rifle,
	Shotgun,
	Rocket,
	GrenadeLauncher,
	ComBall,
	ThrownGrenade
}
