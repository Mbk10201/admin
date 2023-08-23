using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogGoto : LogObject
{
	// Interface implementation
	public override string Name => "Goto";
	public override string Description => "When a admin teleport to a player.";

	/// <summary>
	/// The banned player
	/// </summary>
	[Net] public User Target { get; private set; }

	/// <summary>
	/// The admin player
	/// </summary>
	[Net] public User Admin { get; private set; }

	/// <summary>
	/// The player position
	/// </summary>
	[Net] public Vector3 Position { get; private set; }

	public LogGoto() { }

	public LogGoto( User target, User admin, Vector3 position, string format = "" ) : this()
	{
		Target = target;
		Admin = admin;
		Position = position;

		if( format == "" )
			Format = $"{admin.Name} has teleport to {target.Name}.";
	}
}
