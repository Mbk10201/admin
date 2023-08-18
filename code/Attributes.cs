using System;
using System.ComponentModel.DataAnnotations;

namespace Mbk.Admin;

[AttributeUsage( AttributeTargets.Method )]
public class CommandAttribute : Attribute
{
	public string Key { get; set; }
	public string[] Aliases { get; }
	public string Description { get; set; }
	internal string GeneratedFrom = "";
	internal bool IsGenerated => !string.IsNullOrEmpty( GeneratedFrom );

	public CommandAttribute( string key, params string[] aliases )
	{
		if ( string.IsNullOrEmpty( key ) )
			throw new ArgumentNullException( nameof( key ) );

		Key = key;
		Aliases = aliases;
	}
}

[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
public class PermissionAttribute : Attribute
{
	public string Permission { get; private set; }
	/// <summary>
	/// Dont check for this permission when executing the command
	/// </summary>
	public bool ManualEnforcement { get; init; }
	public PermissionAttribute( string permission, bool manualEnforcement = false )
	{
		Permission = permission;
		ManualEnforcement = manualEnforcement;
	}

	public static implicit operator string( PermissionAttribute attr ) => attr.Permission;
}

