namespace Mbk.Admin;

//
// Summary:
//     Indicates that this panel can be created as a admin page. If we find a Panel
//     in a hud or library assemblies that implements this then we'll try to
//     use it as an admin page panel.
public interface IAdminPage
{
	/// <summary>
	/// The page name to be displayed on the sidenav.
	/// </summary>
	string PageName { get; }

	/// <summary>
	/// The icon to display when the sidenav is not expanded
	/// https://fonts.google.com/icons
	/// </summary>
	string Icon { get; }

	/// <summary>
	/// The HRef (Link) of this page, this is required and must be unique.
	/// </summary>
	string HRef { get; }
}
