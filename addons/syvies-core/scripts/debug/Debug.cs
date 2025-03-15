using Godot;
using System;

namespace SyviesCore.DebugUtils;

public partial class Debug : Node
{
	private static Debug instance;
	private static readonly StringName debugMouseInput = new("debug_mouse");
	private static readonly Key debugMouseKey = Key.F11;


	public override void _EnterTree()
	{
		if (IsInstanceValid(instance))
		{
			GD.PushWarning("Debug instance already exists. Discarding new one.");
			GetParent().RemoveChild(this);
			QueueFree();
			return;
		}
		instance = this;

		CheckForInput();
	}


	public override void _ExitTree()
	{
		if (IsInstanceValid(instance) && instance == this)
		{
			instance = null;
		}
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		if (!Engine.IsEmbeddedInEditor()) { return; }

		if (@event.IsActionPressed(debugMouseInput))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		}
	}


	public static void Assert(bool condition, string message = "")
	{
		if (OS.IsDebugBuild() && condition == false)
		{
			GD.PrintErr("Assertion failed! " + message);
			GD.PushError("Assertion failed! " + message);
			throw new Exception("Assertion failed! " + message);
		}
	}


	private void CheckForInput()
	{
		if (InputMap.HasAction(debugMouseInput)) { return; }

		InputMap.AddAction(debugMouseInput);
		InputEventKey eventKey = new()
		{
			Keycode = debugMouseKey,
		};

		InputMap.ActionAddEvent(debugMouseInput, eventKey);
	}
}
