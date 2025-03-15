using Godot;

namespace SyviesCore.DebugUtils;

public partial class Logger : Node
{
	private static Logger instance;

	private const string PRINT_COLOR = "DARK_GRAY";
	private const string INFO_COLOR = "DODGER_BLUE";
	private const string WARN_COLOR = "GOLD";
	private const string ERROR_COLOR = "ORANGE_RED";
	private static readonly StringName logDebugTitle = new("Last Log");


	public override void _EnterTree()
	{
		if (IsInstanceValid(instance) && instance != this)
		{
			GD.PushWarning("Logger instance already exists. Discarding new one.");
			GetParent().RemoveChild(this);
			QueueFree();
			return;
		}

		instance = this;
	}


	public override void _ExitTree()
	{
		if (IsInstanceValid(instance) && instance == this)
		{
			instance = null;
		}
	}


	private static void SendMessage(string message, int messageTypeIndex = -1, int errorIndex = -1)
	{
		MessageType messageType = messageTypeIndex < 0 ? MessageType.Info : (MessageType)messageTypeIndex;
		Error error = errorIndex < 0 ? Godot.Error.Ok : (Error)errorIndex;

		if (error == Godot.Error.Ok)
		{
			switch (messageType)
			{
				case MessageType.Warning:
					message = $"WARN - {message}";
					break;

				case MessageType.Error:
					message = $"ERR - {message}";
					break;

				case MessageType.Info:
					message = $"INFO - {message}";
					break;

				default:
					break;
			}
		}
		else
		{
			message = $"ERR: {error.ToString().ToUpper()} - {message}";
		}

		if (OS.IsDebugBuild())
		{
			switch (messageType)
			{
				case MessageType.Warning:
					GD.PushWarning(message);
					DebugPanel.AddDebugPropertyRich(logDebugTitle, $"[color={WARN_COLOR}]{message}[/color]", DebugPanel.ORDER_LOG);
					break;

				case MessageType.Error:
					GD.PushError(message);
					DebugPanel.AddDebugPropertyRich(logDebugTitle, $"[color={ERROR_COLOR}]{message}[/color]", DebugPanel.ORDER_LOG);
					break;

				case MessageType.Info:
					GD.PrintRich($"[color={INFO_COLOR}]{message}[/color]");
					DebugPanel.AddDebugPropertyRich(logDebugTitle, $"[color={INFO_COLOR}]{message}[/color]", DebugPanel.ORDER_LOG);
					break;
				
				default:
					GD.PrintRich($"[color={PRINT_COLOR}]{message}[/color]");
					DebugPanel.AddDebugPropertyRich(logDebugTitle, $"[color={PRINT_COLOR}]{message}[/color]", DebugPanel.ORDER_LOG);
					break;
			}
		}
		else if (Engine.IsEditorHint())
		{
			switch (messageType)
			{
				case MessageType.Warning:
					GD.PrintRich($"[color={WARN_COLOR}]{message}[/color]");
					break;

				case MessageType.Error:
					GD.PrintRich($"[color={ERROR_COLOR}]{message}[/color]");
					break;

				case MessageType.Info:
					GD.PrintRich($"[color={INFO_COLOR}]{message}[/color]");
					break;
				
				default:
					GD.PrintRich($"[color={PRINT_COLOR}]{message}[/color]");
					break;
			}
		}

		// TODO - Add user message handling
		// TODO - Add logs
		// message = $"[{Time.GetDatetimeStringFromSystem()}] {message}";
	}


	public static void Print(string message) => instance.CallDeferred(MethodName.SendMessage, message);
	public static void Info(string message) => instance.CallDeferred(MethodName.SendMessage, [message, (int)MessageType.Info]);
	public static void Warning(string message) => instance.CallDeferred(MethodName.SendMessage, [message, (int)MessageType.Warning]);
	public static void Error(string message, Error error = Godot.Error.Failed) => instance.CallDeferred(MethodName.SendMessage, [message, (int)MessageType.Error, (int)error]);


	private enum MessageType
	{
		None,
		Info,
		Warning,
		Error
	}
}
