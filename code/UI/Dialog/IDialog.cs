using Mbk.Admin.Logs;
using Sandbox;

namespace Mbk.Admin.UI.Dialog;

public interface IDialog
{
	/// <summary>
	/// This will auto implement the selected player from the admin players page.
	/// (Optional)
	/// </summary>
	IClient Player { get; set; }

	/// <summary>
	/// This will auto implement the selected command from the admin players page.
	/// (Optional)
	/// </summary>
	Command TargetCommand { get; set; }

	/// <summary>
	/// This will auto implement the selected log from the admin logs page.
	/// (Optional)
	/// </summary>
	LogObject TargetLog { get; set; }
}
