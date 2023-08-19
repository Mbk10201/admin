using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Mbk.Admin;

public partial class Command : BaseNetworkable
{
	/// <summary>
	/// The name of this command.
	/// </summary>
	[Net] public string Name { get; private set; } = "";

	/// <summary>
	/// The parameters list of this command.
	/// </summary>
	[Net, JsonIgnore] public IDictionary<string, string> Params { get; private set; }

	/// <summary>
	/// The icon (Google Material Icons)
	/// </summary>
	[Net, JsonIgnore] public string Icon { get; private set; }

	/// <summary>
	/// Is this command for client targeting (Ex: Ban, Kick, Etc...)
	/// </summary>
	[Net, JsonIgnore] public bool ClientAction { get; private set; } = false;

	[JsonIgnore] public CommandAttribute Attribute { get; set; }
	[JsonIgnore] public MethodDescription Method { get; set; }

	public Command()
	{
		Params = new Dictionary<string, string>();
	}

	public Command( CommandAttribute attribute, MethodDescription method )
	{
		Attribute = attribute;
		Method = method;

		Icon = Attribute.Icon;
		ClientAction = Attribute.ClientAction;

		foreach ( var param in method.Parameters )
		{
			if(!Params.ContainsKey( param.Name ) )
				Params.Add( param.Name, param.ParameterType.Name );
		}
	}

	public ParameterInfo[] GetParameters() => Method.Parameters;

	public static void Load()
	{
		foreach ( var type in TypeLibrary.GetTypes() )
		{
			foreach ( var method in type.Methods )
			{
				LoadMethod( method, type );
			}
		}
	}

	private static void LoadMethod( MethodDescription method, TypeDescription type )
	{
		var attribute = method.GetCustomAttribute<CommandAttribute>();

		if ( attribute == null )
			return;

		Command command = new( attribute, method ) 
		{ 
			Name = attribute.Key
		};

		if ( AdminSystem.Instance.Commands.Contains( command ) )
			return;

		Log.Info( $"[Admin] New command registered: {command.Name}" );
		AdminSystem.RegisterCommand( command );
	}


	[ConCmd.Server( "RunCommand" )]
	public static void Run( string name, string[] args )
	{
		Game.AssertServer();

		foreach ( var arg in args )
			Log.Info( arg );

		var command = AdminSystem.Instance.Commands.SingleOrDefault( x => x.Name == name );

		if ( command == null )
			return;

		var method = command.Method;

		if ( method == null )
			return;

		method.Invoke( null, args );
	}

	public static Command GetRef( string name ) => AdminSystem.Instance.Commands.SingleOrDefault( x => x.Name == name );
}
