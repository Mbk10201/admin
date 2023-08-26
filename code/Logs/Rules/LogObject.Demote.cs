using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogDemote : LogObject
{
	// Interface implementation
	public override string Name => "Demote";
	public override string Description => "When someone has been demoted by an admin";

	// JobChange Event implementation
	[Net] public User Player { get; private set; }
	[Net] public User Admin { get; private set; }

	public LogDemote() {}

	public LogDemote( User player, User admin, string format = "" ) : this()
	{
		Player = player;
		Admin = admin;

		if ( format == "" )
			Format = $"{admin.Name} has demoted {player.Name}.";
	}
}
