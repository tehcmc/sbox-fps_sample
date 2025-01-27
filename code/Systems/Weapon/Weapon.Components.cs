namespace GameTemplate.Weapons;

public partial class Weapon
{
	public T GetComponent<T>() where T : WeaponComponent
	{
		return Components.Get<T>( false );
	}
	public void Initialize()
	{
		foreach ( var component in Components.GetAll<WeaponComponent>() )
		{
			component.Initialize( this );
		}
	}
	protected void SimulateComponents( IClient cl )
	{
		var player = Owner as Player;
		foreach ( var component in Components.GetAll<WeaponComponent>() )
		{
			component.Simulate( cl, player );
		}
	}
	protected void TickComponents( IClient cl )
	{
		var player = Owner as Player;
		foreach ( var component in Components.GetAll<WeaponComponent>() )
		{
			component.Tick( cl, player );
		}
	}

	public void RunGameEvent( string eventName )
	{
		Player?.RunGameEvent( eventName );
	}

	public override void BuildInput()
	{
		foreach ( var component in Components.GetAll<WeaponComponent>() )
		{
			component.BuildInput();
		}
	}
}
