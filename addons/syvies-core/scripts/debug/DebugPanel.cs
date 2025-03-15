using Godot;

namespace SyviesCore.DebugUtils;

public partial class DebugPanel : CanvasLayer
{
	public const int ORDER_LOG = 0;
	public const int ORDER_FPS = 1;

	private static DebugPanel instance;
	private static readonly StringName fps_name = new("FPS");
	private static readonly StringName debugPanelInput = new("debug_panel");
	private static readonly Key debugPanelKey = Key.F12;

	[Export] private BoxContainer propertyContainer;


	public override void _EnterTree()
	{
		if (IsInstanceValid(instance))
		{
			GD.PushWarning("DebugPanel instance already exists. Discarding new one.");
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


	public override void _Ready()
	{
		Debug.Assert(propertyContainer != null, "DebugPanel needs a PropertyContainer.");

		Visible = OS.IsDebugBuild();
	}


	public override void _Process(double delta)
	{
		if (Visible)
		{
			AddDebugProperty(fps_name, Mathf.RoundToInt(1.0 / delta), ORDER_FPS);
		}
	}


	public override void _Input(InputEvent @event)
	{
		if (!IsInstanceValid(instance)) { return; }

		if (@event.IsActionPressed(debugPanelInput) && OS.IsDebugBuild())
		{
			instance.Visible = !instance.Visible;
		}
	}


	public static void AddDebugProperty(StringName title, Variant value, int order = -1)
	{
		if (instance == null) { return; }

		Label target = (Label)instance.propertyContainer.FindChild(title, true, false);

		if (target == null)
		{
			target = new()
			{
				Name = title
			};
			instance.propertyContainer.AddChild(target);
			target.Text = title + ": " + value.ToString();
			instance.propertyContainer.MoveChild(target, order);
		}
		else if (instance.Visible)
		{
			target.Text = title + ": " + value.ToString();
			instance.propertyContainer.MoveChild(target, order);
		}
	}


	public static void AddDebugPropertyRich(StringName title, Variant value, int order = -1)
	{
		if (instance == null) { return; }

		RichTextLabel target = (RichTextLabel)instance.propertyContainer.FindChild(title, true, false);

		if (target == null)
		{
			target = new()
			{
				Name = title,
				BbcodeEnabled = true,
				ScrollActive = false,
			};
			instance.propertyContainer.AddChild(target);
			target.Text = title + ": " + value.ToString();
			instance.propertyContainer.MoveChild(target, order);
		}
		else if (instance.Visible)
		{
			target.Text = title + ": " + value.ToString();
			instance.propertyContainer.MoveChild(target, order);
		}
	}


	private void CheckForInput()
	{
		if (InputMap.HasAction(debugPanelInput)) { return; }

		InputMap.AddAction(debugPanelInput);
		InputEventKey eventKey = new()
		{
			Keycode = debugPanelKey,
		};

		InputMap.ActionAddEvent(debugPanelInput, eventKey);
	}
}
