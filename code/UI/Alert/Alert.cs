using Sandbox;
using System.Linq;

namespace Mbk.Admin.UI.Alert;

public partial class Alert
{
	public static AdminAlertPanel AlertPanel { get; set; }

	[ClientRpc]
	public static void Add( AlertBuilder item )
	{
		AlertPanel.Add( item );
	}

	[ClientRpc]
	public static void Add( string title, string message, eAlertType type )
	{
		AlertPanel.Add( new()
		{
			Title = title,
			Message = message,
			Type = type
		} );
	}

	[GameEvent.Client.Frame]
	public static void Frame()
	{
		if ( Game.RootPanel.Children.OfType<AdminAlertPanel>().Count() == 0 )
		{
			AlertPanel = Game.RootPanel.AddChild<AdminAlertPanel>();
		}
	}
}
