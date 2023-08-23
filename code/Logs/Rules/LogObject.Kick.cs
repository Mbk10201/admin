using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogKick : LogObject
{
	// Interface implementation
	public override string Name => "Kick";
	public override string Description => "When someone has been kicked from the server";

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


	public LogKick() { }

	public LogKick( User target, User admin, string reason, string format = "" ) : this()
	{
		Target = target;
		Admin = admin;
		Reason = reason;

		if ( format == "" )
			Format = $"{admin.Name} has kicked {target.Name} for {reason}.";
	}
}
