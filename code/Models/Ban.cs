using Sandbox;
using System.Linq;

namespace Mbk.Admin.Models;

public partial class Ban : BaseNetworkable
{
	public static Ban IsBan( IClient client ) => AdminSystem.Instance.Bans.SingleOrDefault(x => x.User.SteamId == client.SteamId);
	public static Ban IsBan( long steamid ) => AdminSystem.Instance.Bans.SingleOrDefault(x => x.User.SteamId == steamid );

	[Net] public User User { get; private set; }
	[Net] public User Admin { get; private set; }
	[Net] public string Reason { get; set; }
	[Net] public int Timeout { get; set; }

	public Ban() { }

	public Ban( User user, User admin, string reason, int timeout ) : this()
	{
		User = user;
		Admin = admin;
		Reason = reason;
		Timeout = timeout;
	}
}
