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
	public static IList<Role> GetRolesRefs( this IClient client ) => Role.GetRef(client.GetRoles());

	public static IList<long> GetPermissions( this IClient client )
	{
		IList<long> Permissions = new List<long>();

		foreach( var role in client.GetRolesRefs() ) 
		{ 
			foreach( var perm in role.Permissions )
			{
				if( !Permissions.Contains(perm))
					Permissions.Add(perm);
			}
		}

		return Permissions;
	}

	public static IList<Permission> GetPermissionsRefs( this IClient client ) => Permission.GetRef( GetPermissions(client) );

	public static IList<Command> GetCommandsAccess( this IClient client )
	{
		IList<Command> Permissions = new List<Command>();

		foreach( var perm in GetPermissionsRefs(client))
		{
			foreach( var command in perm.Commands )
			{
				var reference = Command.GetRef( command );

				if( !Permissions.Contains( reference ) )
					Permissions.Add(reference);
			}
		}

		return Permissions;
	}
	//gpublic static IList<long> GetPermissions( this IClient client ) => User.Get( client ).Roles;

	public static bool CanTarget( this IClient client, IClient target )
	{
		return (client.GetHighestRole().ImmunityLevel > target.GetHighestRole().ImmunityLevel);
	}

	public static bool HasCommandAccess( this IClient client, string name ) 
	{
		bool result = false;
		
		foreach(var role in GetRolesRefs(client))
		{
			foreach( var perm in Permission.GetRef(role.Permissions))
			{
				if ( perm.Name == name )
				{
					result = true;
					break;
				}
			}
		}

		return result;
	}
}
