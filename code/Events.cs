using Sandbox;
using System;

namespace Mbk.Admin;

public partial class AdminSystem
{
	public const string OnCommandExecuted = "oncommandexecuted";
	[MethodArguments( new Type[] { typeof( string ) } )]
	public class OnCommandExecutedAttribute : EventAttribute
	{
		public OnCommandExecutedAttribute() : base( OnCommandExecuted ) { }
	}
}
