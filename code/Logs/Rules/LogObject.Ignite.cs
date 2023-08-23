using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogIgnite : LogObject
{
	// Interface implementation
	public override string Name => "Ignite";
	public override string Description => "When someone has been ignited.";

	/// <summary>
	/// The banned player
	/// </summary>
	[Net] public User Target { get; private set; }

	/// <summary>
	/// The admin player
	/// </summary>
	[Net] public User Admin { get; private set; }

	public LogIgnite() { }

	public LogIgnite( User target, User admin, string format = "" ) : this()
	{
		Target = target;
		Admin = admin;

		if( format == "" )
			Format = $"{admin.Name} has ignited {target.Name}.";
	}
}
