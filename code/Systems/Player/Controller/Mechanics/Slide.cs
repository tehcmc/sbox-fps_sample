using System.Reflection.Metadata;

namespace GameTemplate.Mechanics;

/// <summary>
/// Sliding mechanic for players, build up speed on sloped surfaces ala apex legends
/// </summary>
public partial class SlideMechanic : PlayerControllerMechanic
{
	[Net] public bool isSliding { get; set; } = false;
	[Net] public float slideForce { get; set; } = 500f;

	protected override bool ShouldStart()
	{
		if ( !Input.Down( "duck" ) ) return false;

		if ( Player.MoveInput.Length == 0 ) return false;

		return true;
	}
	protected override void Simulate()
	{
		DebugOverlay.ScreenText( "SLIDE!!!!" );
		SlideMovement();
	}
	protected override void OnStart()
	{


	}

	protected virtual void SlideMovement()
	{
		Player.Root.ApplyLocalImpulse( Player.MoveInput.Normal * slideForce );
	}
}
