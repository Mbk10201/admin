using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogJobChange : LogObject
{
	// Interface implementation
	public override string Name => "JobChange";
	public override string Description => "When someone job's changes";

	// JobChange Event implementation
	[Net] public User Player { get; private set; }
	[Net] public User Admin { get; private set; }
	[Net] public string OldJob { get; private set; }
	[Net] public string NewJob { get; private set; }
	[Net] public int OldGrade { get; private set; }
	[Net] public int NewGrade { get; private set; }

	public LogJobChange() {}

	public LogJobChange( User player, User admin, string oldjob, string newjob, int oldgrade, int newgrade, string format = "" ) : this()
	{
		Player = player;
		Admin = admin;
		OldJob = oldjob;
		NewJob = newjob;
		OldGrade = oldgrade;
		NewGrade = newgrade;

		if ( format == "" )
			Format = $"{admin.Name} has changed {player.Name} jobs from {oldgrade} - {oldjob} to {newgrade} - {newjob}.";
	}
}
