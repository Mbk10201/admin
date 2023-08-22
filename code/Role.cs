using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Mbk.Admin;

public partial class Role : BaseNetworkable
{

	/// <summary>
	/// The unique ID of this role.
	/// </summary>
	[Net] public long Id { get; set; }

	/// <summary>
	/// The name of the role.
	/// </summary>
	[Net] public string Name { get; set; }

	/// <summary>
	/// The list of permissions id that this role has.
	/// </summary>
	[Net] public IList<long> Permissions { get; private set; }

	/// <summary>
	/// The immunity level that this grade has
	/// Example if you target a player that has higher immunity than you, you cannot action on him.
	/// Immunity = superiority
	/// </summary>
	[Net] public int ImmunityLevel { get; private set; }

	/// <summary>
	///	The role icon (filepath).
	/// </summary>
	[Net] public string Icon { get; set; }

	/// <summary>
	///	The role color (HEX, RGB, RGBA)
	/// </summary>
	[Net] public string Color { get; set; }

	/// <summary>
	///	Wheter this roel is mentionabe or not (Can be useful in a chat ?)
	/// </summary>
	[Net] public bool Mentionable { get; set; }

	/// <summary>
	/// If this role can be removed
	/// </summary>
	[Net] public bool CanBeRemoved { get; set; } = true;

	/// <summary>
	/// If this role is the default one when a new player join
	/// Warning, there should be only one in the instance !!
	/// </summary>
	[Net] public bool Default { get; set; } = false;

	public Role() 
	{
		Permissions = new List<long>();
	}

	public Role( string name, int immunitylevel = 25, bool canberemoved = true ) : this()
	{
		Id = Game.Random.NextInt64();
		Name = name;
		ImmunityLevel = immunitylevel;
		CanBeRemoved = canberemoved;

		AdminSystem.Instance.Roles.Add( this );
	}

	public void AddPermission(long id )
	{
		Log.Info( id );
		Permissions.Add( id );
	}

	public void RemovePermission( long id )
	{
		Permissions.Remove( id );
	}

	public void SetImmunity(int immunity)
	{
		ImmunityLevel = immunity;
	}

	/// <summary>
	///	Return a list of permissions object from id references of the role.
	/// </summary>
	public IList<Permission> GetPermissionsByRef()
	{
		IList<Permission> list = new List<Permission>();

		foreach(var permission in Permissions )
		{
			var Ref = Permission.GetRef( permission );

			if( Ref is not null)
				list.Add( Ref );
		}

		return list;
	}

	/// <summary>
	///	Return a role object from id reference.
	/// </summary>
	public static Role GetRef( long id ) => AdminSystem.Instance.Roles.SingleOrDefault( x => x.Id == id );

	/// <summary>
	///	Return a role object from a name reference.
	/// </summary>
	public static Role GetRef( string name ) => AdminSystem.Instance.Roles.SingleOrDefault( x => x.Name == name );

	/// <summary>
	///	Return the default role 
	/// </summary>
	public static Role GetDefault( ) => AdminSystem.Instance.Roles.SingleOrDefault( x => x.Default );

	/// <summary>
	///	Return a role object list from a list id reference
	/// </summary>
	public static IList<Role> GetRef( IList<long> list)
	{
		IList<Role> roles = new List<Role>();

		foreach ( var role in list )
			roles.Add( GetRef( role ) );

		return roles;
	}

	/// <summary>
	///	Return a role object list from a list name reference
	/// </summary>
	public static IList<Role> GetRef( IList<string> list )
	{
		IList<Role> roles = new List<Role>();

		foreach ( var role in list )
			roles.Add( GetRef( role ) );

		return roles;
	}

	/// <summary>
	///	Create a role
	/// </summary>
	[ConCmd.Server("createrole")] 
	public static void Create( string name, int immunity = 25 ) 
	{ 
		_ = new Role( name, immunity );
		AdminSystem.SaveRoles();
	}

	/// <summary>
	///	Update a role name
	/// </summary>
	[ConCmd.Server("updatename")] 
	public static void UpdateName( long roleid, string name ) 
	{ 
		GetRef( roleid ).Name = name;
		AdminSystem.SaveRoles();
	}

	/// <summary>
	///	Delete a role by id
	/// </summary>
	[ConCmd.Server("deleterole")] 
	public static void Delete( long roleid ) 
	{ 
		AdminSystem.Instance.Roles.Remove( GetRef( roleid ) );
		AdminSystem.SaveRoles();
	}

	/// <summary>
	///	Delete a role by name
	/// </summary>
	[ConCmd.Server("deleterole")] 
	public static void Delete( string rolename ) 
	{ 
		AdminSystem.Instance.Roles.Remove( GetRef( rolename ) );
		AdminSystem.SaveRoles();
	}

	/// <summary>
	///	Delete a permission from the role by id
	/// </summary>
	[ConCmd.Server("removepermission")] 
	public static void RemovePermission( long roleid, long permid ) 
	{ 
		GetRef( roleid ).RemovePermission( permid );
		AdminSystem.SaveRoles();
	}

	/// <summary>
	///	Add a permission to the role.
	/// </summary>
	[ConCmd.Server("addpermission")] 
	public static void AddPermission( long roleid, long permid ) 
	{ 
		GetRef( roleid ).AddPermission( permid );
		AdminSystem.SaveRoles();
	}
}
