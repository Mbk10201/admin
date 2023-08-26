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
	/// The list of commands thats this permission has.
	/// </summary>
	[Net] public IList<string> Commands { get; private set; }

	public Permission() {}

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

	/// <summary>
	///	Return a permission object by id reference.
	/// </summary>
	public static Permission GetRef( long id ) => AdminSystem.Instance.Permissions.SingleOrDefault( x => x.Id == id );

	/// <summary>
	///	Return a permission object by name reference.
	/// </summary>
	public static Permission GetRef( string name ) => AdminSystem.Instance.Permissions.SingleOrDefault( x => x.Name == name );

	/// <summary>
	///	Return a permission ist object by a list id references.
	/// </summary>
	public static IList<Permission> GetRef( IList<long> list )
	{
		IList<Permission> permissions = new List<Permission>();

		foreach ( var perm in list )
			permissions.Add( GetRef( perm ) );

		return permissions;
	}

	/// <summary>
	///	Create a permission.
	/// </summary>
	[ConCmd.Server("createpermission")] 
	public static void Create( string name, string description = "" ) 
	{ 
		_ = new Permission( name, description );
		AdminSystem.SavePermissions();
	}

	/// <summary>
	///	Update a permission name.
	/// </summary>
	[ConCmd.Server("updatepermissionname")] 
	public static void UpdateName( long permid, string name ) 
	{ 
		GetRef( permid ).Name = name;
		AdminSystem.SavePermissions();
	}

	/// <summary>
	///	Update a permission description.
	/// </summary>
	[ConCmd.Server("updatepermissiondescription")] 
	public static void UpdateDescription( long permid, string description ) 
	{ 
		GetRef( permid ).Description = description;
		AdminSystem.SavePermissions();
	}

	/// <summary>
	///	Delete a permission by id reference.
	/// </summary>
	[ConCmd.Server("deletepermission")] 
	public static void Delete( long permid ) 
	{ 
		foreach( var role in AdminSystem.Instance.Roles )
		{
			role.Permissions.Remove( permid );
		}
		
		AdminSystem.Instance.Permissions.Remove( GetRef( permid ) );
		AdminSystem.SavePermissions();
	}

	/// <summary>
	///	Delete a permission by name reference.
	/// </summary>
	[ConCmd.Server("deletepermission")] 
	public static void Delete( string permname ) 
	{
		var perm = GetRef( permname );
		
		foreach ( var role in AdminSystem.Instance.Roles )
		{
			role.Permissions.Remove( perm.Id );
		}

		AdminSystem.Instance.Permissions.Remove( perm );
		AdminSystem.SaveRoles();
		AdminSystem.SavePermissions();
	}

	/// <summary>
	///	Delete a command access from permission.
	/// </summary>
	[ConCmd.Server( "removecommand" )] 
	public static void RemoveCommand( long permid, string name ) 
	{ 
		GetRef( permid ).RemoveCommand( name );
		AdminSystem.SavePermissions();
	}

	/// <summary>
	///	Add a command access to the permission.
	/// </summary>
	[ConCmd.Server( "addcommand" )] 
	public static void AddCommand( long permid, string name ) 
	{ 
		GetRef( permid ).AddCommand( name );
		AdminSystem.SavePermissions();
	}
}
