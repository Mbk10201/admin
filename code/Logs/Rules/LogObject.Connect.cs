using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogConnect : LogObject
{
	// Interface implementation
	public override string Name => "Connect";
	public override string Description => "When someone has successfully connected to the server";

	// Connect Event implementation
	[Net] public Entity Player { get; private set; }

	public LogConnect() { }

	public LogConnect( Entity player, string format = "" ) : this()
	{
		Player = player;

		if ( format == "" )
			Format = $"{player.Client.Name} has connected.";
	}
}
