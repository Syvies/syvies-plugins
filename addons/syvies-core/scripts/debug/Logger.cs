using System;
using System.Collections.Generic;
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
	private readonly Queue<Action> messageQueue = new();


	public override void _EnterTree()
	{
		if (IsInstanceValid(instance) && instance != this)
		{
			GD.PushWarning("⚠ Logger instance already exists. Discarding new one.");
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


	public override void _Process(double delta)
	{
		if (messageQueue.Count == 0) { return; }

		Action messageCallback = messageQueue.Dequeue();
		messageCallback?.Invoke();
	}


	private static void SendMessage(string message, MessageType messageType = MessageType.None, Error error = Godot.Error.Ok)
	{
		// If we have a Godot.Error then display it, else if we are an error message but the error is Godot.Error.Ok then display "⛆⛆⛆" as the error.
		string errorSeparator = error != Godot.Error.Ok ? $" {error.ToString().ToSnakeCase().ToUpper()} - " : messageType == MessageType.Error ? " ⛆⛆⛆ - " : " - ";

		message = messageType switch
		{
			MessageType.Warning => $"[b]⯁ WARNING{errorSeparator}[/b]{message}",
			MessageType.Error => $"[b]■ ERROR{errorSeparator}[/b]{message}",
			MessageType.Info => $"[b]🞇 INFO{errorSeparator}[/b]{message}",
			_ => $"[b]🞇 PRINT{errorSeparator}[/b]{message}",
		};

 		if (Engine.IsEditorHint())
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
		else if (OS.IsDebugBuild())
		{
			switch (messageType)
			{
				case MessageType.Warning:
					// GD.PushWarning(message);
					GD.PrintRich($"[color={WARN_COLOR}]{message}[/color]");
					DebugPanel.AddDebugPropertyRich(logDebugTitle, $"[color={WARN_COLOR}]{message}[/color]", DebugPanel.ORDER_LOG);
					break;

				case MessageType.Error:
					// GD.PushError(message);
					GD.PrintRich($"[color={ERROR_COLOR}]{message}[/color]");
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

		// TODO - Add user message handling
		// TODO - Add logs
		// message = $"[{Time.GetDatetimeStringFromSystem()}] {message}";
	}


	public static void Print(string message, Error error = Godot.Error.Ok) => SendMessage(message, MessageType.None, error);
	public static void Info(string message, Error error = Godot.Error.Ok) => SendMessage(message, MessageType.Info, error);
	public static void Warning(string message, Error error = Godot.Error.Ok) => SendMessage(message, MessageType.Warning, error);
	public static void Error(string message, Error error = Godot.Error.Failed) => SendMessage(message, MessageType.Error, error);


	public static void ThreadPrint(string message, Error error = Godot.Error.Ok) => instance.messageQueue.Enqueue(() => Print(message, error));
	public static void ThreadInfo(string message, Error error = Godot.Error.Ok) => instance.messageQueue.Enqueue(() => Info(message, error));
	public static void ThreadWarning(string message, Error error = Godot.Error.Ok) => instance.messageQueue.Enqueue(() => Warning(message, error));
	public static void ThreadError(string message, Error error = Godot.Error.Failed) => instance.messageQueue.Enqueue(() => Error(message, error));


	private enum MessageType
	{
		None,
		Info,
		Warning,
		Error
	}


	private static string LogStart()
	{
		return
		"         .'cdkKXWWMMMMWWXKkdc'.         \n" +
		"      .,o0NMMWNK0OkkkkO0KNWMMN0o,.      \n" +
		"    .:ONMMNOo:'..........':oONWMNO:.    \n" +
		"   ,OWMW0l'..'coxO0000Oxoc'..'l0WMWO,   \n" +
		" .lXMWKl. 'o0NWX0kxxxxOKNWN0o' .lKMMXl. \n" +
		".oNMWk' .dXWKd;..      .'ckNWXd. 'OWMNo.\n" +
		"cXMWk. ,0WXd.              ;OWWO, .kWMXc\n" +
		"0MMK, 'OMXc                 .xWMO' ;KMM0\n" +
		"WMMd. oWWd.                  ,KMWl .xWMN\n" +
		"MMWl .xMNc                   .kMMx. lWMM\n" +
		"WMWo .dMWo                   'OMWd. oWMM\n" +
		"XMMO. :XM0,                 .oNMX: .OMMM\n" +
		"xWMNl .oNM0:               .dNMNl. lNMMM\n" +
		"'OMMXc .lXWNk:.          'l0WMM0;.cXMMMM\n" +
		" ,OWMXo. 'dXWWKkdlcccclx0NMMMMMWX0NMMMMM\n" +
		"  .xNMW0c. .:x0XWWMMMMWWX0dldXMMMMMMMMMM\n" +
		"   .:OWMWKd:....,;::::;,....c0WMMMMMMMMM\n" +
		"     .:kXWMWXOdlc;;,,;:cldOXWMMMMMMMMMMM\n" +
		"        .cx0NWMMMMWWWWMMMMMMMMMMMMMMMMWK\n" +
		"           .;oOKNWMMMMMMMMMMMMMMMMMMMNO,";
	}
}
