using System.Reflection.Metadata;

namespace GameTemplate.Mechanics;

/// <summary>
/// Sliding mechanic for players, build up speed on sloped surfaces ala apex legends
/// </summary>
public partial class SlideMechanic : PlayerControllerMechanic
{
	[Net] public bool isSliding { get; set; } = false;
	[Net] public float slideForce { get; set; } = 1500f;

	[Net] public float slideThreshold { get; set; } = 250f;

	protected override bool ShouldStart()
	{
		if ( !Input.Down( "duck" ) ) return false;



		if ( isSliding ) return false;

		if ( Player.MoveInput.Length == 0 ) return false;

		if ( Player.Velocity.Length < slideThreshold ) return false;
		return true;
	}
	protected override void Simulate()
	{

		SlideMovement();
	}
	protected override void OnStart()
	{
		isSliding = true;

	}

	protected virtual void SlideMovement()
	{

		DebugOverlay.ScreenText( $"Momentum:{Player.Velocity.Length}" );
		Player.PhysicsBody.ApplyImpulse( Player.Velocity.Length * slideForce );

	}

	protected override void OnStop()
	{
		DebugOverlay.ScreenText( "SLIDE!!!! stop" );
		isSliding = false;
	}
}
