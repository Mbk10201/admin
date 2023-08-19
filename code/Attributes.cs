using System;
using System.ComponentModel.DataAnnotations;

namespace Mbk.Admin;

[AttributeUsage( AttributeTargets.Method )]
public class CommandAttribute : Attribute
{
	public string Key { get; set; }
	public string Icon { get; set; }
	public bool ClientAction { get; set; }
	public string[] Aliases { get; }
	public string Description { get; set; }
	internal string GeneratedFrom = "";
	internal bool IsGenerated => !string.IsNullOrEmpty( GeneratedFrom );

	public CommandAttribute( string key, string icon = "star", bool clientaction = false, params string[] aliases)
	{
		if ( string.IsNullOrEmpty( key ) )
			throw new ArgumentNullException( nameof( key ) );

		Key = key;
		Icon = icon;
		ClientAction = clientaction;
		Aliases = aliases;
	}
}
