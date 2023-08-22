namespace Mbk.Admin.UI.Alert;

public enum eAlertType
{
	Info,
	Success,
	Error,
	Warning,
	Custom
}

public class AlertBuilder
{
	public string Title { get; set; }
	public string Message { get; set; }
	public eAlertType Type { get; set; }
}
