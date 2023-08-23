using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogItemUsed : LogObject
{
	// Interface implementation
	public override string Name => "Item";
	public override string Description => "When someone use a item";
	public override string Category => "Items";

	// Spawn prop implementation
	[Net] public Entity Player { get; private set; }
	//[Net] public Item Item { get; private set; }

	public LogItemUsed() { }

	/*public LogItemUsed( Entity player, IInventoryItem item, string format = "" ) : this()
	{
		Player = player;
		Item = item;

		if ( format == "" )
			Format = $"{player.Client.Name} has used {item.Name}.";
	}*/
}
