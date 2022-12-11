using Facepunch.Gunfight.CameraModifiers;
using Sandbox;

namespace Facepunch.Gunfight.Mechanics;

/// <summary>
/// The jump mechanic for players.
/// </summary>
public partial class Jump : BaseMechanic
{
	public override int SortOrder => 25;
	public override string Name => "Jump";

	private float Cooldown => 0.5f;
	private float Gravity => 700f;

	private bool Lock = false;
	
	public TimeUntil WindupComplete { get; protected set; }

	protected override bool ShouldActivate()
	{
		if ( Lock ) return true;

		if ( !Input.Pressed( InputButton.Jump ) ) return false;
		if ( !Controller.GroundEntity.IsValid() ) return false;

		return true;
	}

	protected override void OnActivate()
	{
		WindupComplete = 0.1f;
		Lock = true;

		_ = new CameraModifiers.JumpModifier();

		Simulate();
	}

	protected override void OnDeactivate()
	{
		TimeUntilCanNextActivate = Cooldown;
	}

	protected override void Simulate()
	{
		if ( !Controller.GroundEntity.IsValid() )
		{
			Lock = false;
			return;
		}

		if ( WindupComplete && Controller.GroundEntity.IsValid() )
		{
			float flGroundFactor = 1.0f;
			float flMul = 250f;
			float startz = Velocity.z;

			Velocity = Velocity.WithZ( startz + flMul * flGroundFactor );
			Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;

			Controller.GetMechanic<Walk>()
				.ClearGroundEntity();

			Controller.Player.PlaySound( "sounds/player/foley/gear/player.jump.gear.sound" );

			Lock = false;
		}
	}
}
