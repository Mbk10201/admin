namespace Mbk.Admin.Logs;

public partial class LogTest : LogObject
{
	// Interface implementation
	public override string Name => "Test";
	public override string Description => "Debug test";

	public LogTest() { }

	public LogTest( string format = "" ) : base()
	{
		if ( format == "" )
			Format = $"new LogTest.";
	}
}
