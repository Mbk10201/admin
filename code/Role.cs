using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Mbk.Admin;

public partial class Role : BaseNetworkable
{
	/// <summary>
	/// The unique ID of this role
	/// </summary>
	[Net] public long Id { get; set; }

	/// <summary>
	/// The name of the role
	/// </summary>
	[Net] public string Name { get; set; }

	/// <summary>
	/// The list of permissions id that this role has
	/// </summary>
	[Net] public IList<long> Permissions { get; private set; }

	/// <summary>
	/// The immunity level that this grade has
	/// Example if you target a player that has higher immunity than you, you cannot action on him.
	/// Immunity = superiority
	/// </summary>
	[Net] public int ImmunityLevel { get; private set; }

	/// <summary>
	///	The role icon (filepath)
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

	public Role() 
	{
		Permissions = new List<long>();
	}

	public Role( string name, int immunitylevel = 25 ) : this()
	{
		Id = Game.Random.NextInt64();
		Name = name;
		ImmunityLevel = immunitylevel;

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

	public static Role GetRef( long id ) => AdminSystem.Instance.Roles.SingleOrDefault( x => x.Id == id );
	public static Role GetRef( string name ) => AdminSystem.Instance.Roles.SingleOrDefault( x => x.Name == name );

	public static IList<Role> GetRef( IList<long> list)
	{
		IList<Role> roles = new List<Role>();

		foreach ( var role in list )
			roles.Add( GetRef( role ) );

		return roles;
	}

	public static IList<Role> GetRef( IList<string> list )
	{
		IList<Role> roles = new List<Role>();

		foreach ( var role in list )
			roles.Add( GetRef( role ) );

		return roles;
	}

	[ConCmd.Server("createrole")] public static void Create( string name, int immunity = 25 ) { _ = new Role( name, immunity ); }

	[ConCmd.Server("updatename")] public static void UpdateName( long roleid, string name ) { GetRef( roleid ).Name = name; }

	[ConCmd.Server("deleterole")] public static void Delete( long roleid ) { AdminSystem.Instance.Roles.Remove( GetRef( roleid ) ); }
	[ConCmd.Server("deleterole")] public static void Delete( string rolename ) { AdminSystem.Instance.Roles.Remove( GetRef( rolename ) ); }

	[ConCmd.Server("removepermission")] public static void RemovePermission( long roleid, long permid ) { GetRef( roleid ).RemovePermission( permid ); }
	[ConCmd.Server("addpermission")] public static void AddPermission( long roleid, long permid ) { GetRef( roleid ).AddPermission( permid ); }
}
