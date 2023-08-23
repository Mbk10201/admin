using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogKill : LogObject
{
	// Interface implementation
	public override string Name => "Kill";
	public override string Description => "When a kill / suicide has been commited";

	// Kill Event implementation
	[Net] public Entity Victim { get; private set; }
	[Net] public Entity Killer { get; private set; }

	public LogKill() { }

	public LogKill( Entity victim, Entity killer, string format = "" ) : this()
	{
		Victim = victim;
		Killer = killer;

		if ( format == "" )
			Format = $"{killer.Client.Name} has killed {victim.Client.Name}.";
	}
}
