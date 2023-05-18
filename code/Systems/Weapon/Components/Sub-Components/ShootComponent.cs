using GameTemplate.Mechanics;
using Sandbox;

namespace GameTemplate.Weapons;

[Prefab]
public partial class ShootComponent : EntityComponent<Weapon>
{
	protected Weapon Weapon => Entity;
	protected Player Player => Weapon.Owner as Player;

	public virtual void Simulate( IClient cl, Player player )
	{

	}
	public virtual void Tick( IClient cl, Player player )
	{

	}

	public virtual void Fire()
	{

	}
}

