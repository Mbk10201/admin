using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogJobChange : LogObject
{
	// Interface implementation
	public override string Name => "JobChange";
	public override string Description => "When someone job's changes";

	// JobChange Event implementation
	[Net] public Entity Player { get; private set; }
	[Net] public Entity Admin { get; private set; }
	[Net] public int OldJob { get; private set; }
	[Net] public int NewJob { get; private set; }
	[Net] public int OldGrade { get; private set; }
	[Net] public int NewGrade { get; private set; }

	public LogJobChange() {}

	public LogJobChange( Entity player, Entity admin, int oldjob, int newjob, int oldgrade, int newgrade, string format = "" ) : this()
	{
		Player = player;
		Admin = admin;
		OldJob = oldjob;
		NewJob = newjob;
		OldGrade = oldgrade;
		NewGrade = newgrade;

		if ( format == "" )
			Format = $"{admin.Client.Name} has changed {player.Client.Name} jobs from {oldgrade} - {oldjob} to {newgrade} - {newjob}.";
	}
}
