using Godot;
using SyviesCore.Utils;

namespace CameraFlow;

[Tool]
public partial class VirtualCamera : Node3D
{
	public static readonly StringName virtualCameraGroup = new("virtualCamera");

	[Export] public CameraPriority Priority { get; set; } = CameraPriority.Disabled;
	[Export(PropertyHint.ResourceType, "CameraResource")] public CameraResource CameraResource
	{
		get => camResource;
		set
		{
			if (value == camResource) { return; }

			if (IsInstanceValid(camResource))
			{
				camResource.PropertyUpdated -= OnCamResourceChanged;
			}

			camResource = value;

			if (IsInstanceValid(camResource))
			{
				camResource.PropertyUpdated += OnCamResourceChanged;
			}

			OnCamResourceChanged();
		}
	}
	[ExportGroup("Transitions")]
	[Export] public TweenResource TransitionIn { get; set; } = null;
	[Export] public TweenResource TransitionOut { get; set; } = null;

	private CameraResource camResource = new()
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
