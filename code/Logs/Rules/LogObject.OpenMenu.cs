using Sandbox;
using Sandbox.UI;

namespace Mbk.Admin.Logs;

public partial class LogOpenMenu : LogObject
{
	// Interface implementation
	public override string Name => "OpenMenu";
	public override string Description => "When someone open a menu, doesn't matter wich one";

	// Respawn Event implementation
	[Net] public Entity Player { get; private set; }
	[Net] public Panel PanelRef { get; private set; }
	[Net] public string PanelName { get; private set; }

	public LogOpenMenu() { }

	public LogOpenMenu( Entity player, Panel panelref, string format = "" ) : this()
	{
		Player = player;
		PanelRef = panelref;
		PanelName = panelref.ElementName;

		if ( format == "" )
			Format = $"{player.Client.Name} has opened {panelref.ElementName}.";
	}
}
