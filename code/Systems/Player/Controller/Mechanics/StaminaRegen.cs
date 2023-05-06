namespace GameTemplate.Mechanics;


partial class StaminaRegen : PlayerControllerMechanic
{

	public float regenAmount = 0.5f;


	//rate at which stamina will regen (i.e 1f = 1 sec)
	public float regenRate = 0.1f;

	protected override bool ShouldStart()
	{
		if ( Player.Stamina >= 100f ) return false;
		if ( Player.Controller.IsMechanicActive<SprintMechanic>() ) return false;
		return true;
	}

	protected override void Simulate()
	{

		Player.Stamina += regenAmount;
		DebugOverlay.ScreenText( $"Stamina:{Player.Stamina}" );
	}
}
