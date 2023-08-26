using Mbk.Admin.UI.Alert;
using Mbk.Admin.UI.Dialog.Childs;
using Mbk.Discord;
using Mbk.Discord.Attributes;
using Mbk.Discord.Models;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Mbk.Admin;

public partial class User : BaseNetworkable
{
	public static User Get( IClient client ) => AdminSystem.Instance.Users.SingleOrDefault(x => x.SteamId == client.SteamId);
	public static User Get( long steamid ) => AdminSystem.Instance.Users.SingleOrDefault(x => x.SteamId == steamid );

	[Net] public long SteamId { get; private set; }
	[Net] public string Name { get; private set; }
	[Net] public IList<long> Roles { get; private set; }
	[JsonIgnore] public Vector3 LastPosition { get; set; }

	public User()
	{
		Roles = new List<long>();
	}

	public User( long steamid, string name ) : this()
	{
		SteamId = steamid;
		Name = name;
	}

	/// <summary>
	///	Add a role to the self.
	/// </summary>
	[DiscordGameEvent( "Give role", "give_role", "When a admin gives a role to a player." )]
	[Command( "Give Role", typeof( AddRoleDialog ), "fluent:shield-lock-48-regular", clientaction: true )]
	public static void GiveRole( long steamid, long adminid, long roleid )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		var role = Role.GetRef( roleid );

		if ( !client.GetRoles().Contains( roleid ) )
		{
			Get( client ).Roles.Add( roleid );
			AdminSystem.SaveUsers();
			Alert.Add( To.Single( admin ), "Success", $"You have successfully added the role {role.Name} to {client.Name}", eAlertType.Success );

			var EventSettings = DiscordSystem.GetGameEvent( "give_role" );

			if ( EventSettings.Broadcast )
			{
				var message = new MessageForm();

				if ( EventSettings.DisplayEmbed )
				{
					message = new()
					{
						Embeds = new()
						{
							new Embed()
							{
								Title = EventSettings.Name,
								Description = $"{admin.Name} has given {role.Name} role to {client.Name}.",
								Color = EventSettings.GetColor()
							}
						}
					};
				}
				else
				{
					message = new()
					{
						Content = $"{admin.Name} has given {role.Name} role to {client.Name}."
					};
				}

				if ( EventSettings.UseAsBot && Client.Instance.TokenValid )
				{
					if ( EventSettings.ChannelID is null )
						return;

					Client.SendMessage( EventSettings.ChannelID.Value, message );
				}
				else
				{
					if ( EventSettings.Webhook == string.Empty )
						return;

					Webhook.SendMessage( EventSettings.Webhook, message );
				}
			}
		}
		else
			Alert.Add( To.Single( admin ), "Warning", $"{client.Name} already has the role {role.Name}", eAlertType.Warning );
	}

	/// <summary>
	///	Give a role to self.
	/// </summary>
	[ConCmd.Server("GiveSelfRole")]
	public static void GiveRole( string rolename )
	{
		var client = ConsoleSystem.Caller;

		if ( client is not null )
		{
			var role = AdminSystem.Instance.Roles.SingleOrDefault( x => x.Name == rolename);

			if ( role == null )
			{
				Log.Warning( $"[AdminSystem] Role {rolename} doesn't exist" );
				return;
			}

			if ( !client.GetRoles().Contains( role.Id ) )
			{
				Get( client ).Roles.Add( role.Id );
				AdminSystem.SaveUsers();
			}
			else
				Log.Warning( $"[AdminSystem] you already have the role {rolename}" );
		}
	}

	/// <summary>
	///	Kick player
	///	Should be called from the server !
	/// </summary>
	[ClientRpc]
	public static void KickPlayer( )
	{
		Log.Info( "Kicked" );
		
		Game.LocalClient.Kick( );
	}
}
