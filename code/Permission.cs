using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mbk.Admin;

public partial class Permission : BaseNetworkable
{
	/// <summary>
	/// The unique ID of the permission
	/// </summary>
	[Net]
	public long Id { get; set; }

	/// <summary>
	/// The name of the permission
	/// </summary>
	[Net]
	public string Name { get; set; }

	/// <summary>
	/// The name of the permission
	/// </summary>
	[Net]
	public string Description { get; set; }

	/// <summary>
	/// If this permission can be removed
	/// </summary>
	[Net]
	public bool CanBeRemoved { get; set; } = true;

	/// <summary>
	/// The list of commands thats this permission has .
	/// </summary>
	[Net] public IList<string> Commands { get; private set; }

	public Permission() 
	{
	}

	public Permission( string name, string description, bool canberemoved = true ) : this()
	{
		Id = Game.Random.NextInt64();
		Name = name;
		Description = description;
		CanBeRemoved = canberemoved;

		AdminSystem.Instance.Permissions.Add( this );
	}

	public void AddCommand( string name )
	{
		Commands.Add( name );
	}

	public void RemoveCommand( string name )
	{
		Commands.Remove( name );
	}

	public IList<Command> GetCommandsByRef()
	{
		IList<Command> list = new List<Command>();

		foreach ( var command in Commands )
		{
			var Ref = Command.GetRef( command );

			if ( Ref is not null )
				list.Add( Ref );
		}

		return list;
	}

	public static Permission GetRef( long id ) => AdminSystem.Instance.Permissions.SingleOrDefault( x => x.Id == id );
	public static Permission GetRef( string name ) => AdminSystem.Instance.Permissions.SingleOrDefault( x => x.Name == name );

	public static IList<Permission> GetRef( IList<long> list )
	{
		IList<Permission> permissions = new List<Permission>();

		foreach ( var perm in list )
			permissions.Add( GetRef( perm ) );

		return permissions;
	}

	[ConCmd.Server("createpermission")] 
	public static void Create( string name, string description = "" ) 
	{ 
		_ = new Permission( name, description );
		AdminSystem.SavePermissions();
	}

	[ConCmd.Server("updatepermissionname")] 
	public static void UpdateName( long permid, string name ) 
	{ 
		GetRef( permid ).Name = name;
		AdminSystem.SavePermissions();
	}

	[ConCmd.Server("updatepermissiondescription")] 
	public static void UpdateDescription( long permid, string description ) 
	{ 
		GetRef( permid ).Description = description;
		AdminSystem.SavePermissions();
	}

	[ConCmd.Server("deletepermission")] 
	public static void Delete( long permid ) 
	{ 
		AdminSystem.Instance.Permissions.Remove( GetRef( permid ) );
		AdminSystem.SavePermissions();
	}
	[ConCmd.Server("deletepermission")] 
	public static void Delete( string permname ) 
	{ 
		AdminSystem.Instance.Permissions.Remove( GetRef( permname ) );
		AdminSystem.SavePermissions();
	}

	[ConCmd.Server( "removecommand" )] 
	public static void RemoveCommand( long permid, string name ) 
	{ 
		GetRef( permid ).RemoveCommand( name );
		AdminSystem.SavePermissions();
	}

	[ConCmd.Server( "addcommand" )] 
	public static void AddCommand( long permid, string name ) 
	{ 
		GetRef( permid ).AddCommand( name );
		AdminSystem.SavePermissions();
	}
}
