using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogBring : LogObject
{
	// Interface implementation
	public override string Name => "Goto";
	public override string Description => "When a admin bring a player to him.";

	/// <summary>
	/// The banned player
	/// </summary>
	[Net] public User Target { get; private set; }

	/// <summary>
	/// The admin player
	/// </summary>
	[Net] public User Admin { get; private set; }

	/// <summary>
	/// The admin position
	/// </summary>
	[Net] public Vector3 Position { get; private set; }

	public LogBring() { }

	public LogBring( User target, User admin, Vector3 position, string format = "" ) : this()
	{
		Target = target;
		Admin = admin;
		Position = position;

		if( format == "" )
			Format = $"{target.Name} has been teleport to {admin.Name}.";
	}
}
