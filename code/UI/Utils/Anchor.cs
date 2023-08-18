using Sandbox.UI;
using System.Linq;

namespace Mbk.Admin.UI.Utils;

/// <summary>
/// A panel that will navigate to an href but also have .active class if href is active
/// </summary>
[ClassName( "a" )]
[Alias( "navlink", "li")]
public class Anchor : Panel
{
	NavigatorPanel Navigator;
	public string HRef { get; set; }
	public string Match { get; set; }

	public override void OnParentChanged()
	{
		base.OnParentChanged();

		Navigator = Ancestors.OfType<NavigatorPanel>().FirstOrDefault();
	}

	protected override void OnMouseDown( MousePanelEvent e )
	{
		if ( e.Button == "mouseleft" )
		{
			Log.Info( $"Anchor: clicked redirect to ->{HRef}" );
			CreateEvent( "navigate", HRef );
			e.StopPropagation();
		}
	}

	public override void Tick()
	{
		base.Tick();
		var active = Navigator?.CurrentUrlMatches( Match ?? HRef ) ?? false;
		SetClass( "active", active );
	}
}
