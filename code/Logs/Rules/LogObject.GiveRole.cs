using Sandbox;
using System.Text.Json.Serialization;

namespace Mbk.Admin.Logs;

public partial class LogGiveRole : LogObject
{
	// Interface implementation
	public override string Name => "Give role";
	public override string Description => "When a admin give a player role.";

	// Spawn prop implementation
	[Net] public User Target { get; private set; }
	[Net] public User Admin { get; private set; }
	[Net] public Role Role { get; private set; }
	[Net] public string Message { get; private set; }

	public LogGiveRole() { }

	public LogGiveRole( User target, User admin, Role role, string message, string format = "" ) : this()
	{
		Target = target;
		Admin = admin;
		Role = role;
		Message = message;

		if ( format == "" )
			Format = $"{admin.Name} has given {role.Name} to {target.Name}";
	}
}
