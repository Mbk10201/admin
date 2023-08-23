using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogSpawnProp : LogObject
{
	// Interface implementation
	public override string Name => "Spawn prop";
	public override string Description => "When someone spawn a prop";
	public override string Category => "Entities";

	// Spawn prop implementation
	[Net] public Entity Player { get; private set; }
	[Net] public Entity Prop { get; private set; }

	public LogSpawnProp() { }

	public LogSpawnProp( Entity player, Entity prop, string format ) : this()
	{
		Player = player;
		Prop = prop;

		if ( format == "" )
			Format = $"{player.Client.Name} has spawned an {prop.ClassName}.";
	}
}
