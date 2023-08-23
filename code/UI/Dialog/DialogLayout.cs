using Mbk.Admin.Logs;
using Sandbox;
using Sandbox.UI;

namespace Mbk.Admin.UI.Dialog;

[StyleSheet]
public class DialogLayout : Panel
{
	public static DialogLayout Instance { get; private set; }
	public IDialog Current { get; private set; }

	public DialogLayout()
	{
		Instance = this;
	}

	public static IDialog Show<T>(IClient player) where T : IDialog, new()
	{
		T val = new T();
		val.Player = player;
		(val as Panel).Parent = Instance;
		Instance.Current = val;

		return val;
	}

	public static IDialog Show<T>( Command command ) where T : IDialog, new()
	{
		T val = new T();
		val.TargetCommand = command;
		(val as Panel).Parent = Instance;
		Instance.Current = val;

		return val;
	}

	public static IDialog Show<T>( LogObject log ) where T : IDialog, new()
	{
		T val = new T();
		val.TargetLog = log;
		(val as Panel).Parent = Instance;
		Instance.Current = val;

		return val;
	}

	public override void Tick()
	{
		base.Tick();

		if ( ChildrenCount > 0 )
			Style.ZIndex = 3;
		else
			Style.ZIndex = -1;
	}
}
