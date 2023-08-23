using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogRespawn : LogObject
{
	// Interface implementation
	public override string Name => "Respawn";
	public override string Description => "When someone respawn";

	// Respawn Event implementation
	[Net] public Entity Player { get; private set; }
	[Net] public Transform Transform { get; private set; }

	public LogRespawn() { }

	public LogRespawn( Entity player, Transform transform, string format = "" ) : this()
	{
		Player = player;
		Transform = transform;

		if ( format == "" )
			Format = $"{player.Client.Name} has respawned at {transform}.";
	}
}
