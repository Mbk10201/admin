using Sandbox;
using System.Linq;

namespace Mbk.Admin;

public partial class Ban : BaseNetworkable
{
	public static Ban IsBan( IClient client ) => AdminSystem.Instance.Bans.SingleOrDefault(x => x.User.SteamId == client.SteamId);
	public static Ban IsBan( long steamid ) => AdminSystem.Instance.Bans.SingleOrDefault(x => x.User.SteamId == steamid );

	[Net] public User User { get; private set; }
	[Net] public string Reason { get; set; }
	[Net] public int Timeout { get; set; }

	public Ban() { }

	public Ban( User user, string reason, int timeout ) : this()
	{
		User = user;
		Reason = reason;
		Timeout = timeout;
	}

	[ConCmd.Server( "Ban")]
	public static void BanPlayer( long steamid, string reason, int timeout)
	{
		AdminSystem.Instance.Bans.Add( new Ban(User.Get(steamid), reason, timeout ) );
		AdminSystem.SaveBans();
	}

	[ConCmd.Server( "Unban" )]
	public static void UnbanPlayer( long steamid )
	{
		var record = AdminSystem.Instance.Bans.SingleOrDefault( x => x.User.SteamId == steamid );

		if( record != null )
		{
			AdminSystem.Instance.Bans.Remove( record );
			AdminSystem.SaveBans();
		}
	}
}
