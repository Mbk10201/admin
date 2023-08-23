using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogUnmute : LogObject
{
	// Interface implementation
	public override string Name => "Unmute";
	public override string Description => "When someone has been unmuted.";

	/// <summary>
	/// The banned player
	/// </summary>
	[Net] public User Target { get; private set; }

	/// <summary>
	/// The admin player
	/// </summary>
	[Net] public User Admin { get; private set; }

	public LogUnmute() { }

	public LogUnmute( User target, User admin, string format = "" ) : this()
	{
		Target = target;
		Admin = admin;

		if ( format == "" )
			Format = $"{admin.Name} has unmuted {target.Name}.";
	}
}
