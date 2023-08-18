using Sandbox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mbk.Admin;

public static class ClientExtensions
{
	public static bool HasPermission( this IClient client, string permission )
	{
		//return Permission.Has( client, permission );
		return true;
	}

	public static void AddRole( this IClient client, long roleid )
	{
		User.AddRole( client.SteamId, roleid );
	}

	public static Role GetHighestRole( this IClient client ) => Role.GetRef( client.GetRoles() ).MaxBy( x => x.ImmunityLevel );

	public static IList<long> GetRoles( this IClient client ) => User.Get( client ).Roles;

	public static bool CanTarget( this IClient client, IClient target )
	{
		return (client.GetHighestRole().ImmunityLevel > target.GetHighestRole().ImmunityLevel);
	}
}
