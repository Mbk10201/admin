using Sandbox;
using System.Linq;

namespace Mbk.Admin;

public static class Commands
{
	[Command( "kick", "meeting_room" )]
	public static void KickPlayer( long steamid, string reason = "")
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );

		if ( client == null )
			return;

		client.Kick();
	}

	[Command( "ban", "block" )]
	public static void Ban( long steamid, string reason, int timeout )
	{
		AdminSystem.Instance.Bans.Add( new Ban( User.Get( steamid ), reason, timeout ) );
		AdminSystem.SaveBans();
	}

	[Command( "unban", "block" )]
	public static void Unban( long steamid )
	{
		var record = AdminSystem.Instance.Bans.SingleOrDefault( x => x.User.SteamId == steamid );

		if ( record != null )
		{
			AdminSystem.Instance.Bans.Remove( record );
			AdminSystem.SaveBans();
		}
	}

	[Command( "mute", "volume_mute" )]
	public static void Mute( long steamid, string reason = "" )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );

		if ( client == null )
			return;

		client.Kick();
	}

	[Command( "printhello" )]
	public static void CommandHello()
	{
		Log.Info( "Hello world :D" );
	}
}
