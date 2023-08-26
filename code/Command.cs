using Sandbox;
using System;
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
	[Net, JsonIgnore] public bool ClientAction { get; private set; }

	/// <summary>
	/// Allow this command to be displayed in the ui.
	/// </summary>
	[Net, JsonIgnore] public bool DisplayInUI { get; private set; }

	/// <summary>
	/// Is this command for client targeting (Ex: Ban, Kick, Etc...)
	/// </summary>
	[Net, JsonIgnore] public string DialogType { get; private set; }

	/// <summary>
	/// The client who has called this command.
	/// </summary>
	public static IClient Caller { get; set; } = null;

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
		DisplayInUI = Attribute.DisplayInUI;
		DialogType = attribute.DialogType.Name;

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

		AdminSystem.RegisterCommand( command );
	}


	[ConCmd.Server( "RunCommand" )]
	public static void Run( string name, string p1 = "", string p2 = "", string p3 = "", string p4 = "", string p5 = "", string p6 = "" )
	{
		var client = ConsoleSystem.Caller;
		var command = AdminSystem.Instance.Commands.SingleOrDefault( x => x.Name == name );

		if ( command == null )
			return;

		var parameters = new string[] { p1, p2, p3, p4, p5, p6 };
		Execute( command, client, parameters.Where( p => !string.IsNullOrEmpty( p ) ).ToArray() );
	}

	public static void Execute( Command command, IClient client, string[] args)
	{
		var method = command.Method;

		ParameterInfo[] parameters = method.Parameters;
		
		int parameterCount = parameters.Length;
		int requiredParameters = parameterCount - parameters.Count( p => p.IsOptional );

		if ( parameterCount > 0 )
		{
			if ( args == default )
			{
				Log.Info( $"Tried to execute command {command} but it requires arguments!");
				return;
			}

			int argsCount = args.Length;
			if ( parameterCount < argsCount || argsCount < requiredParameters )
			{
				Log.Info( $"Tried to execute command {command.Name} but the parameter count doesn't match the argument count!" );
				return;
			}

			var parameterTypes = parameters.Select( p => p.ParameterType ).ToList();
			var parameterValues = new object[parameterCount];
			for ( int i = 0; i < parameterCount; i++ )
			{
				var type = parameterTypes[i];

				//Debug.Log( $"Element {i} in {argsCount}" );
				if ( i >= argsCount )
				{
					//Debug.Log( $"Using default value" );
					parameterValues[i] = parameters[i].DefaultValue;
					continue;
				}

				string arg = args[i];
				var value = ParseType( client, type, arg );
				if ( value == null )
				{
					//Logging.TellClient( caller, $"Tried to execute command {command} but the argument {arg} couldn't be converted to {type}!", MessageType.Error );
					return;
				}

				parameterValues[i] = value;
			}

			method.Invoke( null, parameterValues );
		}
		else
		{
			method.Invoke( null, null );
		}
	}

	private static object ParseType( IClient caller, Type type, string arg )
	{
		if ( type.IsAssignableFrom( arg.GetType() ) )
		{
			return arg;
		}

		object value = null;
		try
		{
			value = Convert.ChangeType( arg, type );
			//Debug.Log( $"Converted {arg} to type {type}" );
		}
		catch ( InvalidCastException )
		{
			// We cant cast with the builtin parsers, try custom ones

			bool parsed = false;

			var parsers = GetParsers( type );
			var parserCount = parsers == default ? 0 : parsers.Length;
			if ( parsers == null || parserCount == 0 )
			{
				//Debug.Log( $"No parsers found for type {type}!" );
				return null;
			}

			//Debug.Log( $"Found {parserCount} parsers for type {type}!" );
			foreach ( var parser in parsers )
			{
				//Debug.Log( $"Trying out parser {parser}" );
				value = parser.Parse( caller, arg );
				if ( value != default && type.IsAssignableFrom( value.GetType() ) )
				{
					//Debug.Log( $"Successfully parsed {arg} to {value}!" );
					parsed = true;
					break;
				}
			}

			if ( !parsed )
			{
				return null;
			}
		}

		return value;
	}

	private static ICommandParser[] GetParsers( Type t )
	{
		return TypeLibrary.GetTypes()
				.Where( td =>
					td.Interfaces.Any(
						i => i.IsAssignableTo( typeof( ICommandParser ) )
							&& i.FullName?.Contains( t.Name ) == true
					)
				)
				.Select( td => td.Create<ICommandParser>() )
				.ToArray();
	}

	/// <summary>
	/// Parses input into object of specified type
	/// </summary>
	/// <typeparam name="T">Type to parse to object into</typeparam>
	public interface ICommandParser<T> : ICommandParser
	{
		public new T Parse( IClient caller, string input );
	}

	/// <summary>
	/// Parses input into some object
	/// </summary>
	public interface ICommandParser
	{
		// This second interface is required to be able to handle these objects with sbox reflection.
		// Related issue: https://github.com/sboxgame/issues/issues/2881
		public object Parse( IClient caller, string input );
	}

	public static Command GetRef( string name ) => AdminSystem.Instance.Commands.SingleOrDefault( x => x.Name == name );
}
