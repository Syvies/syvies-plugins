using Godot;
using SyviesCore.DebugUtils;
using SyviesCore.Utils;

namespace CameraFlow;

[Tool]
public partial class ThirdPersonCamera : Marker3D
{
	[ExportGroup("Third Person Settings")]
	[Export(PropertyHint.Range, "0,10,,or_greater")] private float Distance
	{
		get => distanceValue;
		set
		{
			if (value == distanceValue) { return; }

			distanceValue = Mathf.Max(value, 0f);

			if (IsInstanceValid(springArm))
			{
				springArm.SpringLength = distanceValue;
			}
		}
	}
	[Export] private float DefaultAngle
	{
		get => defaultAngleValue;
		set
		{
			if (value == defaultAngleValue) { return; }

			defaultAngleValue = Mathf.Clamp(value, MinPitch, MaxPitch);
		}
	}
	[Export(PropertyHint.Range, "0,90,")] private float MaxPitch
	{
		get => maxPitchValue;
		set
		{
			if (value == maxPitchValue) { return; }

			maxPitchValue = Mathf.Clamp(value, 0f, 90f);
			DefaultAngle = defaultAngleValue;
		}
	}
	[Export(PropertyHint.Range, "-90,0,")] private float MinPitch
	{
		get => minPitchValue;
		set
		{
			if (value == minPitchValue) { return; }

			minPitchValue = Mathf.Clamp(value, -90f, 0f);
			DefaultAngle = defaultAngleValue;
		}
	}
	[ExportGroup("Virtual Camera Settings")]
	[Export] private CameraPriority Priority
	{
		get
		{
			if (IsInstanceValid(virtualCamera))
			{
				return virtualCamera.Priority;
			}
			return CameraPriority.Disabled;
		}
		set => ChangePriority(value);
	}
	[Export] private CameraResource CameraResource
	{
		get
		{
			if (IsInstanceValid(virtualCamera))
			{
				return virtualCamera.CameraResource;
			}
			return null;
		}
		set
		{
			if (IsInstanceValid(virtualCamera))
			{
				if (value == virtualCamera.CameraResource) { return; }
				virtualCamera.CameraResource = value;
			}
		}
	}
	[ExportSubgroup("Transitions")]
	[Export] public TweenResource TransitionIn
	{
		get
		{
			if (IsInstanceValid(virtualCamera))
			{
				return virtualCamera.TransitionIn;
			}
			return null;
		}
		set
		{
			if (IsInstanceValid(virtualCamera))
			{
				if (value == virtualCamera.TransitionIn) { return; }
				virtualCamera.TransitionIn = value;
			}
		}
	}
	[Export] public TweenResource TransitionOut
	{
		get
		{
			if (IsInstanceValid(virtualCamera))
			{
				return virtualCamera.TransitionOut;
			}
			return null;
		}
		set
		{
			if (IsInstanceValid(virtualCamera))
			{
				if (value == virtualCamera.TransitionOut) { return; }
				virtualCamera.TransitionOut = value;
			}
		}
	}

	private Node3D cameraTarget;
	private Node3D yRotator;
	private Node3D xRotator;
	private SpringArm3D springArm;
	private VirtualCamera virtualCamera;
	private float distanceValue = 5f;
	private float defaultAngleValue = -10f;
	private float maxPitchValue = 60f;
	private float minPitchValue = -70f;


	public override void _EnterTree()
	{
		cameraTarget = new();
		yRotator = new();
		xRotator = new();
		springArm = new();
		virtualCamera = new();

		springArm.AddChild(virtualCamera);
		xRotator.AddChild(springArm);
		yRotator.AddChild(xRotator);
		cameraTarget.AddChild(yRotator);
		AddChild(cameraTarget);
	}


	public override void _ExitTree()
	{
		springArm.RemoveChild(virtualCamera);
		xRotator.RemoveChild(springArm);
		yRotator.RemoveChild(xRotator);
		cameraTarget.RemoveChild(yRotator);
		RemoveChild(cameraTarget);

		cameraTarget.QueueFree();
		yRotator.QueueFree();
		xRotator.QueueFree();
		springArm.QueueFree();
		virtualCamera.QueueFree();
	}


	public override void _Ready()
	{
		if (!Engine.IsEditorHint())
		{
			SetProcess(false);

			DefaultAngle = defaultAngleValue;
			springArm.SpringLength = distanceValue;

			virtualCamera.Position = Vector3.Zero;
			xRotator.Rotation = Vector3.Right * Mathf.DegToRad(DefaultAngle);
		}
	}


	public override void _Process(double delta)
	{
		if (IsInstanceValid(xRotator) && IsInstanceValid(virtualCamera))
		{
			virtualCamera.Position = Vector3.Back * springArm.SpringLength;
			xRotator.Rotation = Vector3.Right * Mathf.DegToRad(Mathf.Clamp(DefaultAngle, MinPitch, MaxPitch));
		}
	}


	public void AimCamera(Vector2 input)
	{
		AddYaw(input.X);
		AddPitch(input.Y);
		ClampPitch();
	}


	public void ChangePriority(CameraPriority newPriority)
	{
		if (IsInstanceValid(virtualCamera))
		{
			virtualCamera.Priority = newPriority;
		}
	}


	public Vector2 GetAimDirection()
	{
		return Vector2.FromAngle(-(Mathf.Pi / 2f + yRotator.GlobalRotation.Y));
	}


	public Transform3D GetAimTransform()
	{
		return virtualCamera.GlobalTransform;
	}


	private void AddYaw(float input)
	{
		if (Mathf.IsZeroApprox(input)) { return; }

		yRotator.RotateY(Mathf.DegToRad(input));
	}


	private void AddPitch(float input)
	{
		if (Mathf.IsZeroApprox(input)) { return; }

		xRotator.RotateX(Mathf.DegToRad(input));
	}


	private void ClampPitch()
	{
		Vector3 rotation = xRotator.Rotation;
		rotation.X = Mathf.Clamp(rotation.X, Mathf.DegToRad(MinPitch), Mathf.DegToRad(MaxPitch));
		xRotator.Rotation = rotation;
	}
}
