using Sandbox;

namespace Mbk.Admin.UI.Dialog;

public interface IDialog
{
	/// <summary>
	/// This will auto implement the selected player from the admin players page.
	/// </summary>
	IClient Player { get; set; }
}
