using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogDisconnect : LogObject
{
	// Interface implementation
	public override string Name => "Disconnect";
	public override string Description => "When someone has successfully disconnected from the server";

	// Disconnect Event implementation
	[Net] public IClient Player { get; private set; }

	public LogDisconnect() { }

	public LogDisconnect( IClient player, string format = "" ) : this()
	{
		Player = player;

		if ( format == "" )
			Format = $"{player.Client.Name} has disconnected.";
	}
}
