using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogSetHealth : LogObject
{
	// Interface implementation
	public override string Name => "Set Health";
	public override string Description => "When a admin change the health of a player.";

	/// <summary>
	/// The target player
	/// </summary>
	[Net] public User Target { get; private set; }

	/// <summary>
	/// The admin player
	/// </summary>
	[Net] public User Admin { get; private set; }

	/// <summary>
	/// The value
	/// </summary>
	[Net] public float Value { get; private set; }

	public LogSetHealth() { }

	public LogSetHealth( User target, User admin, float value, string format = "" ) : this()
	{
		Target = target;
		Admin = admin;
		Value = value;

		if ( format == "" )
			Format = $"{admin.Name} has set the health of {target.Name} to {value}.";
	}
}
