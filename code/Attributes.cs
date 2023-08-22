using Mbk.Admin.UI.Dialog;
using Mbk.Admin.UI.Dialog.Childs;
using System;
using System.ComponentModel.DataAnnotations;

namespace Mbk.Admin;

[AttributeUsage( AttributeTargets.Method )]
public class CommandAttribute : Attribute
{
	public string Key { get; set; }
	public string Icon { get; set; }
	public bool ClientAction { get; set; }
	public bool DisplayInUI { get; set; }
	public string[] Aliases { get; }
	public string Description { get; set; }
	public Type DialogType { get; set; }
	internal string GeneratedFrom = "";
	internal bool IsGenerated => !string.IsNullOrEmpty( GeneratedFrom );

	public CommandAttribute( string key, Type dialog, string icon = "material-symbols:360", bool clientaction = false, bool displayinui = true, params string[] aliases)
	{
		if ( string.IsNullOrEmpty( key ) )
			throw new ArgumentNullException( nameof( key ) );

		Key = key;
		Icon = icon;
		ClientAction = clientaction;
		Aliases = aliases;
		DialogType = dialog;
		DisplayInUI = displayinui;
	}
}
