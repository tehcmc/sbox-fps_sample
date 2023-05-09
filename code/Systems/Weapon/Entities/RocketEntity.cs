//todo --- add AI?? to make rocket move towards set point. no pathfinding, just move towards x at y speed
// add timer,damage,speed values
//other stuff ---- 


[Prefab, Title( "Weapon Entity" ), Icon( "track_changes" )]
public partial class Rocket : AnimatedEntity
{
	[Net, Prefab] public float rocketTime { get; set; } = 10f; // time before rocket explodes if it hasn't hit something yet

	[Net, Prefab] public float rocketSpeed { get; set; } = 500f;
	[Net, Predicted] Vector3 TargetPos { get; set; }

	[ClientRpc]
	public virtual void Destroy()
	{
		Delete();
	}

	public virtual void MoveTowards( Vector3 target )
	{
		this.Position += (target - this.Position).Normal * rocketSpeed * Time.Delta;
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

	}


}
