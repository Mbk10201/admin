using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogReturn : LogObject
{
	// Interface implementation
	public override string Name => "Return player";
	public override string Description => "When a admin teleport back a player to position before the bring.";

	/// <summary>
	/// The banned player
	/// </summary>
	[Net] public User Target { get; private set; }

	/// <summary>
	/// The admin player
	/// </summary>
	[Net] public User Admin { get; private set; }

	/// <summary>
	/// The player last position
	/// </summary>
	[Net] public Vector3 LastPosition { get; private set; }

	/// <summary>
	/// The player current position
	/// </summary>
	[Net] public Vector3 CurrentPosition { get; private set; }

	public LogReturn() { }

	public LogReturn( User target, User admin, Vector3 lastposition, Vector3 currentposition, string format = "" ) : this()
	{
		Target = target;
		Admin = admin;
		LastPosition = lastposition;
		CurrentPosition = currentposition;

		if( format == "" )
			Format = $"{admin.Name} has teleport back {target.Name}.";
	}
}
