using Sandbox;

namespace Mbk.Admin;

//
// Summary:
//     Indicates that this panel can be created as a admin players page category
public interface IAdminPlayerCategory
{
	/// <summary>
	/// The category name to be displayed on the category header.
	/// </summary>
	string CategoryName { get; }

	/// <summary>
	/// This will auto implement the selected player from the admin players page.
	/// </summary>
	IClient Player { get; set; }
}
