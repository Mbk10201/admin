﻿using Mbk.Admin.Logs;
using Mbk.Admin.Models;
using Mbk.Admin.UI;
using Sandbox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mbk.Admin;

[Library]
[Display( Name = "Admin system" ), Category( "MBK" ), Icon( "admin_panel_settings" )]
public partial class AdminSystem : Entity
{
	public static AdminSystem Instance { get; private set; }
	private static BaseFileSystem fs => FileSystem.Data;
	const string ROLESFILE = "admin_roles.json";
	const string PERMISSIONFILE = "admin_permissions.json";
	const string USERFILE = "admin_users.json";
	const string BANFILE = "admin_bans.json";
	const string LOGFILE = "admin_logs.json";
	const string TESTLOGFILE = "admin_testlogs.json";

	[Net] public IList<Role> Roles { get; set; }

	[Net] public IList<Permission> Permissions { get; set; }

	[Net] public IList<Command> Commands { get; set; }

	[Net] public IList<User> Users { get; set; }

	[Net] public IList<Ban> Bans { get; set; }

	[Net] public IList<IClient> MutedClients { get; set; }

	[Net] public IList<LogObject> Logs { get; set; }

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

	public static void SaveLogs()
	{

		JsonSerializerOptions options = new JsonSerializerOptions
		{
			ReadCommentHandling = JsonCommentHandling.Skip,
			//PropertyNameCaseInsensitive = true,
			//AllowTrailingCommas = true,
			//WriteIndented = true,
			//Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};

		fs.WriteAllText( LOGFILE, JsonSerializer.Serialize<object>( Instance.Logs, options ) );
	}

	public AdminSystem()
	{
		Instance = this;

		if ( Game.IsServer )
		{
			Configure();
		}
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

		Commands = new List<Command>();
		Commands?.Clear();

		Users = new List<User>();
		Users?.Clear();

		Bans = new List<Ban>();
		Bans?.Clear();

		Logs = new List<LogObject>();
		Logs?.Clear();

		Command.Load();

		if ( !fs.FileExists( ROLESFILE ) )
		{
			_ = new Role( "Superadmin", 99, false );
			_ = new Role( "Member", 1, false )
			{
				Default = true
			};

			fs.WriteJson( ROLESFILE, Roles );
		}
		else
			Roles = fs.ReadJsonOrDefault( ROLESFILE, new List<Role>() );

		if ( !fs.FileExists( PERMISSIONFILE ) )
		{
			_ = new Permission( "Management", "Ban / Kick / Mute / Slay", false );
			_ = new Permission( "Fun", "Slap / Freeze / Ignite / Slap", false );
			_ = new Permission( "Teleportation", "Goto / Bring / Return", false );
			_ = new Permission( "Noclip", "Allow to set noclip to players", false);
			_ = new Permission( "Spectate", "Allow to spectate players", false );
			_ = new Permission( "Set Name", "Allow to rename players", false);
			_ = new Permission( "Edit Roles", "Allow to edit roles (Only < immunity)", false );

			foreach(var perm in Permissions)
			{
				Roles.Single( x => x.Name == "Superadmin" ).Permissions.Add( perm.Id );
			}
			SaveRoles();

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

		if ( !fs.FileExists( LOGFILE ) )
			fs.WriteJson( LOGFILE, Logs );
		else
			Logs = fs.ReadJsonOrDefault( LOGFILE, new List<LogObject>() );
	}

	public static void Toggle() => AdminUI.Instance.Toggle();

	public static void RegisterCommand( Command command ) => Instance.Commands.Add( command );

	[GameEvent.Client.Frame]
	public void Frame()
	{
		if ( Game.RootPanel.Children.OfType<AdminUI>().Count() == 0 )
			Game.RootPanel.AddChild<AdminUI>();
	}

	[ConCmd.Client( "openadmin" )]
	public static void OpenAdmin()
	{
		AdminSystem.Toggle();
	}

	[GameEvent.Server.ClientJoined]
	static void ClientJoin( ClientJoinedEvent ev )
	{
		var client = ev.Client;

		var user = Instance.Users.FirstOrDefault( x => x.SteamId == client.SteamId );

		if ( user is null )
		{
			User userItem = new User( client.SteamId, client.Name );
			userItem.Roles.Add( Role.GetDefault().Id );

			Instance.Users.Add( userItem );
			AdminSystem.SaveUsers();
			Log.Info( "[AdminSystem] new user detected & added" );
		}
		else
		{
			var record = Ban.IsBan( user.SteamId );
			
			if ( record != null )
			{
				Log.Info( record );
				Log.Info( client );

				client.Kick();
			}
		}
	}

	[ConCmd.Server]
	public static void WriteLog( LogObject log )
	{
		Game.AssertServer();

		Instance.Logs.Add( log );
		SaveLogs();

		Event.Run( OnLogWrite, log );
	}

	public const string OnLogWrite = "onlogwrite";

	[MethodArguments( new Type[] { typeof( LogObject ) } )]
	public class OnLogWriteAttribute : EventAttribute
	{
		public OnLogWriteAttribute() : base( OnLogWrite ) { }
	}
}
