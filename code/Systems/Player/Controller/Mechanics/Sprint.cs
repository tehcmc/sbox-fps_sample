namespace GameTemplate.Mechanics;

/// <summary>
/// The basic sprinting mechanic for players.
/// It shouldn't, though.
/// </summary>
public partial class SprintMechanic : PlayerControllerMechanic
{
	/// <summary>
	/// Sprint has a higher priority than other mechanics.
	/// </summary>
	public override int SortOrder => 10;
	public override float? WishSpeed => 820f;

	public float drainAmount = 1f;


	protected override bool ShouldStart()
	{
		if ( !Input.Down( "run" ) ) return false;

		if ( Player.MoveInput.Length == 0 ) return false;
		if ( Player.Stamina <= 0 ) return false;
		return true;
	}

	protected override void Simulate()
	{

		Math.Clamp( (Player.Stamina -= drainAmount), 0, 100 );


		DebugOverlay.ScreenText( $"Stamina:{Player.Stamina}" );

	}
}
