using Sandbox;
using System;
using System.Linq;

namespace Mbk.Admin.Logs;

public abstract partial class LogObject : BaseNetworkable
{
	[Net] public Guid Id { get; set; }
	public virtual string Name => "Not defined";
	public virtual string Description => "Description not defined";
	public virtual string Category => "General";
	[Net] public long Timestamp { get; set; } = 0;
	[Net] public string Format { get; set; }

	public LogObject()
	{
		Id = Guid.NewGuid();
		Timestamp = new DateTimeOffset( DateTime.UtcNow ).ToUnixTimeSeconds();
	}

	public static LogObject GetLogRef( Guid id ) => AdminSystem.Instance.Logs.SingleOrDefault( x => x.Id == id );
}

public interface ILog
{
	Guid Id { get; }
	string Name { get; }
	string Description { get; }
	string Category { get; }
	long Timestamp { get; }
	string Format { get; }
}
