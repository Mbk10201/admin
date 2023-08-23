using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogUnban : LogObject
{
	// Interface implementation
	public override string Name => "Unban";
	public override string Description => "When someone has been unbanned from the server";

	/// <summary>
	/// The banned player
	/// </summary>
	[Net] public User Target { get; private set; }

	/// <summary>
	/// The admin player
	/// </summary>
	[Net] public User Admin { get; private set; }

	public LogUnban() { }

	public LogUnban( User target, User admin, string format = "" ) : this()
	{
		Target = target;
		Admin = admin;

		if( format == "" )
			Format = $"{admin.Name} has unbanned {target.Name}.";
	}
}
