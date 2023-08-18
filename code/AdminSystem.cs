using Mbk.Admin.UI;
using Sandbox;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Mbk.Admin;

[Library]
[Display( Name = "Admin system" ), Category( "Globals" ), Icon( "admin_panel_settings" )]
public partial class AdminSystem : Entity
{
	public static AdminSystem Instance { get; private set; }
	private static BaseFileSystem fs => FileSystem.Data;
	const string ROLESFILE = "admin_roles.json";
	const string PERMISSIONFILE = "admin_permissions.json";
	const string USERFILE = "admin_users.json";
	const string BANFILE = "admin_bans.json";

	[Net] public IList<Role> Roles { get; set; }

	[Net] public IList<Permission> Permissions { get; set; }

	[Net] public IList<CommandAttribute> Commands { get; set; }

	[Net] public IList<User> Users { get; set; }

	[Net] public IList<Ban> Bans { get; set; }

	public AdminSystem()
	{
		Instance = this;

		if ( Game.IsServer )
		{
			Configure();
		}
	}

	public static void SaveRoles()
	{
		fs.WriteJson( ROLESFILE, Instance.Roles );
	}

	public static void SavePermissions()
	{
		fs.WriteJson( PERMISSIONFILE, Instance.Permissions );
	}

	public static void SaveUsers()
	{
		fs.WriteJson( USERFILE, Instance.Users );
	}

	public static void SaveBans()
	{
		fs.WriteJson( BANFILE, Instance.Bans );
	}

	[GameEvent.Entity.PostSpawn]
	public static void Initialize()
	{
		Game.AssertServer();
		_ = new AdminSystem();
	}

	public override void ClientSpawn()
	{
		base.Spawn();
	}

	public override void Spawn()
	{
		Transmit = TransmitType.Always;
		base.Spawn();
	}

	public void Configure()
	{
		Roles = new List<Role>();
		Roles?.Clear();

		Permissions = new List<Permission>();
		Permissions?.Clear();

		Commands = new List<CommandAttribute>();
		Commands?.Clear();

		Users = new List<User>();
		Users?.Clear();

		Bans = new List<Ban>();
		Bans?.Clear();

		if ( !fs.FileExists( ROLESFILE ) )
		{
			_ = new Role( "Superadmin", 99 );
			_ = new Role( "Admin", 80 );
			_ = new Role( "Moderator", 50 );

			fs.WriteJson( ROLESFILE, Roles );
		}
		else
			Roles = fs.ReadJsonOrDefault( ROLESFILE, new List<Role>() );

		if ( !fs.FileExists( PERMISSIONFILE ) )
		{
			_ = new Permission( "Kick", "Allow to kick players", false );
			_ = new Permission( "Ban", "Allow to ban players", false );
			_ = new Permission( "Mute", "Allow to mute players", false );
			_ = new Permission( "Go to", "Allow to self teleport to players", false );
			_ = new Permission( "Bring", "Allow to bring players", false );
			_ = new Permission( "Return", "Allow to return back players at last position before the bring or teleport", false );
			_ = new Permission( "Slay", "Allow to slay players", false );
			_ = new Permission( "Noclip", "Allow to set noclip to players", false );
			_ = new Permission( "Spectate", "Allow to spectate players", false );
			_ = new Permission( "Set Name", "Allow to rename players", false );
			_ = new Permission( "Slap", "Allow to slap players", false );
			_ = new Permission( "Freeze", "Allow to freeze players", false );
			_ = new Permission( "Ignite", "Allow to ignite players", false );
			_ = new Permission( "Edit Roles", "Allow to edit roles (Only < immunity)", false );

			fs.WriteJson( PERMISSIONFILE, Permissions );
		}
		else
			Permissions = fs.ReadJsonOrDefault( PERMISSIONFILE, new List<Permission>() );

		if ( !fs.FileExists( USERFILE ) )
			fs.WriteJson( USERFILE, Users );
		else
			Users = fs.ReadJsonOrDefault( USERFILE, new List<User>() );

		if ( !fs.FileExists( BANFILE ) )
			fs.WriteJson( BANFILE, Bans );
		else
			Bans = fs.ReadJsonOrDefault( BANFILE, new List<Ban>() );
	}

	public static void Toggle() => AdminUI.Instance.Toggle();

	[GameEvent.Client.Frame]
	public void Frame()
	{
		if ( Game.RootPanel.Children.OfType<AdminUI>().Count() == 0 )
			Game.RootPanel.AddChild<AdminUI>();
	}

	[GameEvent.Server.ClientJoined]
	static void ClientJoin( ClientJoinedEvent ev )
	{
		var client = ev.Client;

		var user = Instance.Users.FirstOrDefault( x => x.SteamId == client.SteamId );

		if ( user is null )
		{
			Instance.Users.Add( new(client.SteamId, client.Name));
			Log.Info( "[AdminSystem] new user detected & added" );
			SaveUsers();
		}
		else
		{
			var record = Ban.IsBan( user.SteamId );
			
			if ( record != null )
			{
				Log.Info( record );
				Log.Info( client );

				User.Kick( To.Single( client ) );
			}
		}
	}
}
