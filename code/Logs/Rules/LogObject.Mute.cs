using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogMute : LogObject
{
	// Interface implementation
	public override string Name => "Mute";
	public override string Description => "When someone has been muted.";

	/// <summary>
	/// The banned player
	/// </summary>
	[Net] public User Target { get; private set; }

	/// <summary>
	/// The admin player
	/// </summary>
	[Net] public User Admin { get; private set; }

	/// <summary>
	/// The reason of the penalty
	/// </summary>
	[Net] public string Reason { get; private set; }

	/// <summary>
	/// The duration of the penalty
	/// </summary>
	[Net] public int Duration { get; private set; }

	public LogMute() { }

	public LogMute( User target, User admin, string reason, int duration, string format = "" ) : this()
	{
		Target = target;
		Admin = admin;
		Reason = reason;
		Duration = duration;

		if ( format == "" )
			Format = $"{admin.Name} has muted {target.Name} for {reason} until {duration}.";
	}
}
