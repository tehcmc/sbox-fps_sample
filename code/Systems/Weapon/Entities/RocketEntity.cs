//todo --- add AI?? to make rocket move towards set point. no pathfinding, just move towards x at y speed
// add timer,damage,speed values
//other stuff ---- 


[Prefab, Title( "Weapon Entity" ), Icon( "track_changes" )]
public partial class Rocket : AnimatedEntity
{
	[Net, Prefab] public float rocketTime { get; set; } = 2f; // time before rocket explodes if it hasn't hit something yet

	[Net, Prefab] public float rocketSpeed { get; set; } = 1500f;


	[Net, Prefab, ResourceType( "sound" )] public string MoveSound { get; set; }

	[Net, Prefab, ResourceType( "vpcf" )] public string RocketTrail { get; set; }

	[Net, Prefab] public float explosionRadius { get; set; } = 50f;

	[Net, Prefab] public float explosionDamage { get; set; } = 50f;
	[Net, Predicted] public TimeSince TimeSinceSpawned { get; protected set; }
	[Net, Predicted] public Vector3 TargetPos { get; set; }


	[Net] Sound soundRef { get; set; }

	[Net] bool beingDestroyed { get; set; } = false;
	public override void Spawn()
	{
		base.Spawn();

		var particle = Particles.Create( RocketTrail, this );
		soundRef = PlaySound( MoveSound, Model.Name );
		TimeSinceSpawned = 0;
	}
	public virtual void Destroy()
	{
		beingDestroyed = true;
		soundRef.Stop();
		Explode();
		Delete();
	}

	public virtual void MoveTowards( Vector3 target )
	{

		Velocity = (target - Position).Normal * rocketSpeed * Time.Delta;

	}


	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		var targetRotation = Rotation.LookAt( Velocity, Vector3.Up );
		Position += Velocity;
		LocalRotation = targetRotation;


		TraceResult tr = Trace.Ray( Position, Position + targetRotation.Forward * 50f ).WorldAndEntities().Ignore( this ).Run();


		if ( tr.Hit )
		{
			if ( Game.IsServer )
			{
				Destroy();
			}
		}

		if ( TimeSinceSpawned >= rocketTime || (TimeSinceSpawned > 0.01f && Velocity.Length <= 0) )
		{
			if ( Game.IsServer )
			{
				Destroy();
			}
		}
	}

	void Explode()
	{
		new ExplosionEntity
		{
			Position = Position,
			Radius = explosionRadius,
			Damage = explosionDamage,
			ForceScale = 1000f
		}.Explode( this );

	}

	public bool IsBeingDestroyed()
	{
		return beingDestroyed;
	}
}
