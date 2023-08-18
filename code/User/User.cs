using Sandbox;
using System.Collections.Generic;
using System.Linq;
using static Sandbox.Gizmo;
using static Sandbox.VideoWriter;

namespace Mbk.Admin;

public partial class User : BaseNetworkable
{
	public static User Get( IClient client ) => AdminSystem.Instance.Users.SingleOrDefault(x => x.SteamId == client.SteamId);
	public static User Get( long steamid ) => AdminSystem.Instance.Users.SingleOrDefault(x => x.SteamId == steamid );

	[Net] public long SteamId { get; private set; }
	[Net] public string Name { get; set; }
	[Net] public IList<long> Roles { get; private set; }

	public User()
	{
		Roles = new List<long>();
	}

	public User( long steamid, string name ) : this()
	{
		SteamId = steamid;
		Name = name;
	}

	[ConCmd.Server]
	public static void AddRole( long roleid )
	{
		var client = ConsoleSystem.Caller;

		if ( client is not null )
		{
			if ( !client.GetRoles().Contains( roleid ) )
			{
				Get( client ).Roles.Add( roleid );
				AdminSystem.SaveUsers();
			}
			else
				Log.Warning( $"[AdminSystem] you already have the role {roleid}" );
		}
	}

	[ConCmd.Server("AddRoleByName")]
	public static void AddRole( string playername, long roleid )
	{
		var client = Game.Clients.SingleOrDefault(x => x.Name == playername );

		if ( client is not null )
		{
			if ( !client.GetRoles().Contains( roleid ) )
			{
				Get( client ).Roles.Add( roleid );
				AdminSystem.SaveUsers();
			}
			else
				Log.Warning( $"[AdminSystem] {client.Name} already have the role {roleid}" );
		}
	}

	[ConCmd.Server( "AddRoleBySteamID" )]
	public static void AddRole( long steamid, long roleid )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );

		if( client is not null)
		{
			if ( !client.GetRoles().Contains( roleid ) )
			{
				Get( client ).Roles.Add( roleid );
				AdminSystem.SaveUsers();
			}
			else
				Log.Warning( $"[AdminSystem] {client.Name} you already have the role {roleid}" );
		}
	}

	[ClientRpc]
	public static void Kick( )
	{
		Game.LocalClient.Kick( );
	}
}
