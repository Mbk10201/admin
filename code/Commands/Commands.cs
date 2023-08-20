using Sandbox;
using Sandbox.Internal;
using System.Linq;

namespace Mbk.Admin;

public static class Commands
{
	[Command( "kick", "meeting_room", true)]
	public static void KickPlayer( long steamid, string reason = "")
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );

		if ( client == null )
			return;

		client.Kick();
	}

	[Command( "ban", "block", clientaction: true )]
	public static void Ban( long steamid, string reason, int timeout )
	{
		AdminSystem.Instance.Bans.Add( new Ban( User.Get( steamid ), reason, timeout ) );
		AdminSystem.SaveBans();
	}

	[Command( "unban", "block", clientaction: true )]
	public static void Unban( long steamid )
	{
		var record = AdminSystem.Instance.Bans.SingleOrDefault( x => x.User.SteamId == steamid );

		if ( record != null )
		{
			AdminSystem.Instance.Bans.Remove( record );
			AdminSystem.SaveBans();
		}
	}

	[Command( "mute", "volume_mute", clientaction: true )]
	public static void Mute( long steamid, string reason = "" )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );

		if ( client == null )
			return;

		client.Kick();
	}

	[ConCmd.Server, Command( "goto", "flight_takeoff", clientaction: true )]
	public static void Goto( long targetid )
	{
		var client = ConsoleSystem.Caller;
		var target = Game.Clients.SingleOrDefault( x => x.SteamId == targetid );

		if ( target == null )
			return;

		if ( client.Pawn == null )
			return;

		client.Pawn.Position = target.Position + Vector3.Up * 100;
	}

	[ConCmd.Server, Command( "bring", "flight_land", clientaction: true )]
	public static void Bring( long targetid, string reason = "" )
	{
		var client = ConsoleSystem.Caller;
		var target = Game.Clients.SingleOrDefault( x => x.SteamId == targetid );

		if ( target == null )
			return;

		if ( client.Pawn == null )
			return;

		client.Pawn.Position = target.Position + Vector3.Up * 100;
	}

	[Command( "printhello", clientaction: true )]
	public static void CommandHello()
	{
		Log.Info( Command.Caller );
		
		Log.Info( "Hello world :D" );
	}
}
