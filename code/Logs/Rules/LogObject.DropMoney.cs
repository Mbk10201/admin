using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogDropMoney : LogObject
{
	// Interface implementation
	public override string Name => "Drop Money";
	public override string Description => "When someone drop money";

	// Spawn prop implementation
	[Net] public Entity Player { get; private set; }
	[Net] public int Amount { get; private set; }

	public LogDropMoney() { }

	public LogDropMoney( Entity player, int amount, string format = "" ) : this()
	{
		Player = player;
		Amount = amount;

		if ( format == "" )
			Format = $"{player.Client.Name} has drop {amount}€.";
	}
}
