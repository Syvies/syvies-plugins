using Godot;
using SyviesCore.Utils;

namespace CameraFlow;

[Tool]
public partial class VirtualCamera : Node3D
{
	public static readonly StringName virtualCameraGroup = new("virtualCamera");

	[Export] public CameraPriority Priority
	{
		get => priorityValue;
		set
		{
			if (value == priorityValue) { return; }

			priorityValue = value;
			CameraManager.PriorityUpdated();
		}
	}
	[Export] public CameraResource CameraResource
	{
		get => camResourceValue;
		set
		{
			if (value == camResourceValue) { return; }

			if (IsInstanceValid(camResourceValue))
			{
				camResourceValue.PropertyUpdated -= OnCamResourceChanged;
			}

			camResourceValue = value;

			if (IsInstanceValid(camResourceValue))
			{
				camResourceValue.PropertyUpdated += OnCamResourceChanged;
			}

			OnCamResourceChanged();
		}
	}
	[ExportGroup("Transitions")]
	[Export] public TweenResource TransitionIn { get; set; } = null;
	[Export] public TweenResource TransitionOut { get; set; } = null;

	private CameraPriority priorityValue = CameraPriority.Disabled;
	private CameraResource camResourceValue = new()
	{
		ResourceName = "CameraResource",
	};


	public override void _EnterTree()
	{
		AddToGroup(virtualCameraGroup);
		CameraManager.AddVirtualCamera(this);
	}


	public override void _ExitTree()
	{
		CameraManager.RemoveVirtualCamera(this);
	}


	private void OnCamResourceChanged()
	{
		UpdateGizmos();
	}
}
