using Sandbox;
using System;
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

	public static Permission GetRef( long id ) => AdminSystem.Instance.Permissions.SingleOrDefault( x => x.Id == id );
	public static Permission GetRef( string name ) => AdminSystem.Instance.Permissions.SingleOrDefault( x => x.Name == name );

	[ConCmd.Server("createpermission")] public static void Create( string name, string description = "" ) { _ = new Permission( name, description ); }
	[ConCmd.Server("updatepermissionname")] public static void UpdateName( long permid, string name ) { GetRef( permid ).Name = name; }
	[ConCmd.Server("updatepermissiondescription")] public static void UpdateDescription( long permid, string description ) { GetRef( permid ).Description = description; }

	[ConCmd.Server("deletepermission")] public static void Delete( long permid ) { AdminSystem.Instance.Permissions.Remove( GetRef( permid ) ); }
	[ConCmd.Server("deletepermission")] public static void Delete( string permname ) { AdminSystem.Instance.Permissions.Remove( GetRef( permname ) ); }
}
