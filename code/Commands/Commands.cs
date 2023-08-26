using Mbk.Admin.Models;
using Mbk.Admin.UI;
using Mbk.Admin.UI.Dialog.Childs;
using Mbk.Discord;
using Mbk.Discord.Attributes;
using Mbk.Discord.Models;
using Mbk.Admin.UI.Alert;
using Sandbox;
using System.Linq;
using System.Threading.Tasks;
using Mbk.Admin.Logs;

namespace Mbk.Admin;

public static partial class Commands
{
	[DiscordGameEvent( "Client Kicked", "client_kick", "When a client has been kicked from the server." )]
	[Command( "Kick", typeof(KickDialog), "mdi:exit-run", clientaction: true)]
	public static void KickPlayer( long steamid, long adminid, string reason = "")
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( steamid == adminid )
		{
			Alert.Add( To.Single( admin ), "Impossible", "You cannot kick yourself !", eAlertType.Error );
			return;
		}
		else if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		if ( client == null )
			return;

		Alert.Add( To.Single( admin ), "Success", $"You have successfully kicked {client.Name}", eAlertType.Success );
		AdminSystem.WriteLog( new LogKick(User.Get( client ), User.Get( admin ), reason ) );
		client.Kick();

		var EventSettings = DiscordSystem.GetGameEvent( "client_kick" );

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
							Description = $"{client.Name} has been kicked from the server by {admin.Name} for {reason}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"{client.Name} has been kick from the server by {admin.Name} for {reason}."
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

	[DiscordGameEvent( "Client Banned", "client_ban", "When a client has been banned from the server." )]
	[Command( "Ban", typeof(BanDialog), "material-symbols:do-not-disturb-on-outline", clientaction: true )]
	public static void Ban( long steamid, long adminid, string reason, int timeout )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( steamid == adminid )
		{
			Alert.Add( To.Single( admin ), "Impossible", "You cannot ban yourself !", eAlertType.Error );
			return;
		}
		else if(!admin.CanTarget(client))
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		AdminSystem.Instance.Bans.Add( new Ban( User.Get( steamid ), User.Get( adminid ), reason, timeout ) );
		AdminSystem.SaveBans();
		AdminSystem.WriteLog( new LogBan( User.Get( client ), User.Get( admin ), reason, timeout ) );

		client.Kick();

		Alert.Add( To.Single( admin ), "Success", $"You have successfully banned {client.Name}", eAlertType.Success );

		var EventSettings = DiscordSystem.GetGameEvent( "client_ban" );

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
							Description = $"{client.Name} has been banned from the server by {admin.Name} for {reason} until {timeout}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"{client.Name} has been banned from the server by {admin.Name} for {reason} until {timeout}."
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

	[DiscordGameEvent( "Client Unbanned", "client_unban", "When a client has been unbanned from the server." )]
	[Command( "Unban", typeof( NotImplementedDialog ), "material-symbols:do-not-disturb-off-outline", clientaction: true, displayinui: false )]
	public static void Unban( long steamid, long adminid )
	{
		var record = AdminSystem.Instance.Bans.SingleOrDefault( x => x.User.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( record != null )
		{
			AdminSystem.Instance.Bans.Remove( record );
			AdminSystem.SaveBans();
			AdminSystem.WriteLog( new LogUnban( User.Get(steamid), User.Get(admin) ) );

			Alert.Add( To.Single( admin ), "Success", $"You have successfully unbanned {record.User.Name}", eAlertType.Success );

			var EventSettings = DiscordSystem.GetGameEvent( "client_unban" );

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
								Description = $"{record.User.Name} has been unbanned from the server by {admin.Name}.",
								Color = EventSettings.GetColor()
							}
						}
					};
				}
				else
				{
					message = new()
					{
						Content = $"{record.User.Name} has been unbanned from the server by {admin.Name}."
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
	}

	[DiscordGameEvent( "Client Muted", "client_mute", "When a client has been muted." )]
	[Command( "Mute", typeof( MuteDialog ), "octicon:mute-24", clientaction: true )]
	public static void Mute( long steamid, long adminid, string reason = "", int timeout = 0)
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( client == null )
			return;

		if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		if ( client.IsMuted() )
		{
			Alert.Add( To.Single( admin ), "Warning", $"{client.Name} is already muted !", eAlertType.Warning );
			return;
		}

		AdminSystem.Instance.MutedClients.Add( client );
		AdminSystem.WriteLog( new LogMute( User.Get(client), User.Get(admin), reason, timeout ) );
		Alert.Add( To.Single( admin ), "Success", $"You have successfully muted {client.Name}", eAlertType.Success );

		var EventSettings = DiscordSystem.GetGameEvent( "client_mute" );

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
							Description = $"{client.Name} has been muted by {admin.Name} for {reason}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"{client.Name} has been muted by {admin.Name} for {reason}.",
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

	[DiscordGameEvent( "Client Unmuted", "client_unmute", "When a client has been unmuted." )]
	[Command( "Unmute", typeof( NotImplementedDialog ), "octicon:unmute", clientaction: true )]
	public static void Unmute( long steamid, long adminid )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( client == null )
			return;

		if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		if ( !client.IsMuted( ) )
		{
			Alert.Add( To.Single( admin ), "Warning", $"{client.Name} is not muted !", eAlertType.Warning );
			return;
		}

		AdminSystem.WriteLog( new LogUnmute( User.Get(client), User.Get(admin) ) );
		Alert.Add( To.Single( admin ), "Success", $"You have successfully unmuted {client.Name}", eAlertType.Success );

		var EventSettings = DiscordSystem.GetGameEvent( "client_unmute" );

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
							Description = $"{client.Name} has been unmuted by {admin.Name}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"{client.Name} has been unmuted by {admin.Name}.",
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

	[DiscordGameEvent( "Client goto", "goto_client", "When a admin teleport self to a client." )]
	[ConCmd.Server, Command( "Goto", typeof( NotImplementedDialog ), "game-icons:teleport", clientaction: true )]
	public static void Goto( long steamid, long adminid )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( client == null )
			return;

		if ( client.Pawn == null )
			return;

		if ( steamid == adminid )
		{
			Alert.Add( To.Single( admin ), "Impossible", "You cannot go to yourself !", eAlertType.Error );
			return;
		}
		else if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		admin.Pawn.Position = client.Pawn.Position + Vector3.Up * 100;
		Sound.FromScreen( "teleport" );

		AdminSystem.WriteLog( new LogGoto( User.Get( client ), User.Get( admin ), client.Pawn.Position ) );
		Alert.Add( To.Single( admin ), "Success", $"You have successfully teleported to {client.Name}", eAlertType.Success );

		var EventSettings = DiscordSystem.GetGameEvent( "goto_client" );

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
							Description = $"{admin.Name} has teleported to {client.Name}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"{admin.Name} has teleported to {client.Name}.",
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

	[DiscordGameEvent( "Client bring", "bring_client", "When a admin teleport a client." )]
	[Command( "Bring", typeof( NotImplementedDialog ), "game-icons:teleport", clientaction: true )]
	public static void Bring( long steamid, long adminid )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if( steamid == adminid )
		{
			Alert.Add( To.Single( admin ), "Impossible", "You cannot bring yourself !", eAlertType.Error );
			return;
		}
		else if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		if ( client == null )
			return;

		if ( client.Pawn == null )
			return;

		User.Get(client).LastPosition = client.Position;
		client.Pawn.Position = admin.Pawn.Position + Vector3.Up * 100;
		Sound.FromScreen( "teleport" );

		AdminSystem.WriteLog( new LogBring( User.Get( client ), User.Get( admin ), admin.Pawn.Position ) );
		Alert.Add( To.Single( admin ), "Success", $"You have successfully bringed {client.Name}", eAlertType.Success );

		var EventSettings = DiscordSystem.GetGameEvent( "bring_client" );

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
							Description = $"{client.Name} has been bring to {admin.Name}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"{client.Name} has been bring to {admin.Name}.",
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

	[DiscordGameEvent( "Return back client", "return_back_client", "When a admin send back the client to last position." )]
	[Command( "Return", typeof( NotImplementedDialog ), "game-icons:teleport", clientaction: true )]
	public static void ReturnBack( long steamid, long adminid )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( steamid == adminid )
		{
			Alert.Add( To.Single( admin ), "Impossible", "You cannot return yourself !", eAlertType.Error );
			return;
		}
		else if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		if ( client == null )
			return;

		if ( client.Pawn == null )
			return;

		if ( User.Get( client ).LastPosition == Vector3.Zero )
			return;

		var lastpos = User.Get( client ).LastPosition;
		
		Sound.FromScreen( "teleport" );
		AdminSystem.WriteLog( new LogReturn( User.Get( client ), User.Get( admin ), client.Pawn.Position, lastpos ) );
		Alert.Add( To.Single( admin ), "Success", $"You have successfully returned {client.Name}", eAlertType.Success );

		client.Pawn.Position = lastpos;
		User.Get( client ).LastPosition = Vector3.Zero;

		var EventSettings = DiscordSystem.GetGameEvent( "return_back_client" );

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
							Description = $"{admin.Name} has returned back {client.Name}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"{admin.Name} has returned back {client.Name}."
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

	[DiscordGameEvent( "Client ignite", "client_ignite", "When a admin ignite a player." )]
	[Command( "Ignite", typeof( NotImplementedDialog ), "gridicons:fire", clientaction: true )]
	public static void Ignite( long steamid, long adminid )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( client == null )
			return;

		if ( client.Pawn == null )
			return;

		if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		var particlesys = Cloud.ParticleSystem( "https://asset.party/sugmagaming/fireparticle" );

		var particle = new ParticleSystemEntity()
		{
			ParticleSystemName = particlesys.Name,
			Parent = client.Pawn as Entity
		};

		AdminSystem.WriteLog( new LogIgnite( User.Get( client ), User.Get( admin )) );
		Alert.Add( To.Single( admin ), "Success", $"You have successfully ignited {client.Name}", eAlertType.Success );

		var EventSettings = DiscordSystem.GetGameEvent( "client_ignite" );

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
							Description = $"{client.Name} has been ignited by {admin.Name}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"{client.Name} has been ignited by {admin.Name}.",
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

	[DiscordGameEvent( "Client freeze", "client_freeze", "When a admin freeze a player." )]
	[Command( "Freeze", typeof( NotImplementedDialog ), "ion:snow", clientaction: true )]
	public static void Freeze( long steamid, long adminid )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( client == null )
			return;

		if ( client.Pawn == null )
			return;

		if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		var entity = client.Pawn as AnimatedEntity;
		entity.SetupPhysicsFromModel( PhysicsMotionType.Static );

		Alert.Add( To.Single( admin ), "Success", $"You have successfully freezed {client.Name}", eAlertType.Success );

		var EventSettings = DiscordSystem.GetGameEvent( "client_freeze" );

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
							Description = $"{client.Name} has been freezed by {admin.Name}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"{client.Name} has been freezed by {admin.Name}.",
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

	[DiscordGameEvent( "Client unfreeze", "client_unfreeze", "When a admin unfreeze a player." )]
	[Command( "Unfreeze", typeof( NotImplementedDialog ), "ion:snow", clientaction: true )]
	public static void Unfreeze( long steamid, long adminid )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( client == null )
			return;

		if ( client.Pawn == null )
			return;

		if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		var entity = client.Pawn as AnimatedEntity;
		entity.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		Alert.Add( To.Single( admin ), "Success", $"You have successfully unfreezed {client.Name}", eAlertType.Success );

		var EventSettings = DiscordSystem.GetGameEvent( "client_unfreeze" );

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
							Description = $"{client.Name} has been unfreezed by {admin.Name}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"{client.Name} has been unfreezed by {admin.Name}.",
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

	[DiscordGameEvent( "Client health", "client_health", "When a admin changes a players health." )]
	[Command( "Set Health", typeof( SetHealthDialog ), "akar-icons:health", clientaction: true )]
	public static void SetHealth( long steamid, long adminid, float value )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( client == null )
			return;

		if ( client.Pawn == null )
			return;

		if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		var pawn = client.Pawn as Entity;
		pawn.Health = value;

		AdminSystem.WriteLog( new LogSetHealth( User.Get(client), User.Get( admin ), value ) );
		Alert.Add( To.Single( admin ), "Success", $"You have successfully set {client.Name} health to {value}", eAlertType.Success );

		var EventSettings = DiscordSystem.GetGameEvent( "client_health" );

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
							Description = $"{admin.Name} has set {client.Name} health to {value}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"{admin.Name} has set {client.Name} health to {value}.",
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

	[DiscordGameEvent( "Client damage", "client_damage", "When a admin gives damages to a player." )]
	[Command( "Give damage", typeof( GiveDamageDialog ), "akar-icons:health", clientaction: true )]
	public static void GiveDamage( long steamid, long adminid, float damage )
	{
		var client = Game.Clients.SingleOrDefault( x => x.SteamId == steamid );
		var admin = Game.Clients.SingleOrDefault( x => x.SteamId == adminid );

		if ( client == null )
			return;

		if ( client.Pawn == null )
			return;

		if ( !admin.CanTarget( client ) )
		{
			Alert.Add( To.Single( admin ), "Impossible", $"{client.Name} has more immunity than you !", eAlertType.Error );
			return;
		}

		var pawn = client.Pawn as Entity;
		pawn.TakeDamage( new DamageInfo()
		{
			Attacker = admin.Pawn as Entity,
			Damage = damage
		} );

		AdminSystem.WriteLog( new LogDamage( client.Pawn as Entity, admin.Pawn as Entity, damage) );

		Alert.Add( To.Single( admin ), "Success", $"You have successfully given {damage} damages to {client.Name}.", eAlertType.Success );

		var EventSettings = DiscordSystem.GetGameEvent( "client_damage" );

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
							Description = $"You have successfully given {damage} damages to {client.Name}.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"You have successfully given {damage} damages to {client.Name}."
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

	[DiscordGameEvent( "Reboot", "server_reboot", "When the server reboot." )]
	[Command( "Reboot", typeof( RebootDialog ), "arcticons:simplereboot" )]
	public static async void Reboot( float seconds )
	{
		var EventSettings = DiscordSystem.GetGameEvent( "server_reboot" );

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
							Description = $"The server gonna reboot in {seconds} seconds.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"The server gonna reboot in {seconds} seconds."
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

		await GameTask.DelayRealtimeSeconds( seconds );

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
							Description = "The server has been rebooted.",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = "The server has been rebooted."
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

		GameManager.Current.Shutdown();
	}

	[DiscordGameEvent( "Hostname", "server_hostname", "When the server name has been changed." )]
	[Command( "Hostname", typeof( HostnameDialog ), "solar:server-2-bold-duotone" )]
	public static void Hostname( string name )
	{
		var EventSettings = DiscordSystem.GetGameEvent( "server_hostname" );

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
							Description = $"The server hostname has changed to {name}",
							Color = EventSettings.GetColor()
						}
					}
				};
			}
			else
			{
				message = new()
				{
					Content = $"The server hostname has changed to {name}"
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

		ConsoleSystem.Run( $"hostname {name}" );
	}

	[Command( "Print Hello", typeof( NotImplementedDialog ) )]
	public static void CommandHello()
	{
		Log.Info( Command.Caller );
		
		Log.Info( "Hello world :D" );
	}

	[GameEvent.Client.BuildInput]
	public static void BuildInput()
	{
		var client = Game.LocalClient;

		if(Input.Pressed(InputButton.Voice))
		{
			if ( client.IsMuted() )
				Input.ClearButton( InputButton.Voice );
		}
	}
}
