using Sandbox;
using System.Text.Json.Serialization;

namespace Mbk.Admin.Logs;

public partial class LogChat : LogObject
{
	// Interface implementation
	public override string Name => "Chat message";
	public override string Category => "Roleplay";
	public override string Description => "When someone write a message on the chat";

	// Spawn prop implementation
	[Net] public Entity Player { get; private set; }
	[Net] public string Message { get; private set; }

	public LogChat() { }

	public LogChat( Entity player, string message, string format = "" ) : this()
	{
		Player = player;
		Message = message;

		if ( format == "" )
			Format = $"{player.Client.Name} has writted {message}.";
	}
}
