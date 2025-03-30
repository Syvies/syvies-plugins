using Godot;
using SyviesCore.DebugUtils;
using SyviesCore.Utils;

namespace CameraFlow;

[Tool]
public partial class StrategyCamera : Marker3D
{
	private static readonly Curve defaultLinearCurve = ResourceLoader.Load<Curve>("res://addons/camera-flow/resources/default_linear_curve.tres");
	private static readonly NodePath positionProperty = new("position");

	[Export(PropertyHint.Range, "0,10,,or_greater")] private float DefaultDistance
	{
		get => distanceValue;
		set
		{
			if (value == distanceValue) { return; }

			distanceValue = Mathf.Clamp(value, MinDistance, MaxDistance);
		}
	}
	[Export(PropertyHint.Range, "-90,90,")] private float DefaultAngle
	{
		get => defaultAngleValue;
		set
		{
			if (value == defaultAngleValue) { return; }

			defaultAngleValue = Mathf.Clamp(value, MinPitch, MaxPitch);
		}
	}
	[ExportGroup("Zoom")]
	[Export] private ZoomBehavior zoomBehavior = ZoomBehavior.Distance;
	[Export(PropertyHint.Range, "0,1,,")] public float Zoom
	{
		get => zoomValue;
		set
		{
			value = Mathf.Clamp(value, 0f, 1f);

			if (value == zoomValue) { return; }

			zoomValue = value;

			if (!Engine.IsEditorHint())
			{
				ZoomCamera();
			}
		}
	}
	[ExportSubgroup("Distance")]
	[Export(PropertyHint.Range, "0,1,,or_greater")] private double distanceZoomDuration = 0.2;
	[Export(PropertyHint.Range, "0,10,,or_greater")] private float MaxDistance
	{
		get => maxDistanceValue;
		set
		{
			if (value == maxDistanceValue) { return; }

			maxDistanceValue = Mathf.Clamp(value, MinDistance, float.MaxValue);
			DefaultDistance = distanceValue;
		}
	}
	[Export(PropertyHint.Range, "0,10,,or_greater")] private float MinDistance
	{
		get => minDistanceValue;
		set
		{
			if (value == minDistanceValue) { return; }

			minDistanceValue = Mathf.Clamp(value, 0f, MaxDistance);
			DefaultDistance = distanceValue;
		}
	}
	[Export] private Curve distanceCurve;
	[ExportSubgroup("Size")]
	[Export(PropertyHint.Range, "0.001,100,,or_greater")] public float MaxSize
	{
		get => maxSizeValue;
		set
		{
			if (value == maxSizeValue) { return; }

			maxSizeValue = Mathf.Clamp(value, MinSize, float.MaxValue);
			CameraResource.OrthographicSize = Mathf.Clamp(CameraResource.Size, MinSize, MaxSize);
		}
	}
	[Export(PropertyHint.Range, "0.001,100,,or_greater")] public float MinSize
	{
		get => minSizeValue;
		set
		{
			if (value == minSizeValue) { return; }

			minSizeValue = Mathf.Clamp(value, 0f, MaxSize);
			CameraResource.OrthographicSize = Mathf.Clamp(CameraResource.Size, MinSize, MaxSize);
		}
	}
	[Export] private Curve sizeCurve;
	[ExportGroup("Rotation")]
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
	private VirtualCamera virtualCamera;
	private Tween distanceTween;
	private float zoomValue = 0.1f;
	private float distanceValue = 5f;
	private float minDistanceValue = 1f;
	private float maxDistanceValue = 10f;
	private float minSizeValue = 1f;
	private float maxSizeValue = 10f;
	private float defaultAngleValue = -10f;
	private float maxPitchValue = 60f;
	private float minPitchValue = -70f;
	private Vector4 zoomDistanceCurveRanges = Vector4.Zero;
	private Vector4 zoomSizeCurveRanges = Vector4.Zero;
	private float zoomDistanceRange = 0f;
	private float zoomSizeRange = 0f;


	public StrategyCamera()
	{
		if (!IsInstanceValid(distanceCurve))
		{
			distanceCurve = (Curve)defaultLinearCurve.Duplicate();
		}

		if (!IsInstanceValid(sizeCurve))
		{
			sizeCurve = (Curve)defaultLinearCurve.Duplicate();
		}
	}


	public override void _Ready()
	{
		if (!Engine.IsEditorHint())
		{
			SetProcess(false);

			DefaultAngle = defaultAngleValue;
			DefaultDistance = distanceValue;

			virtualCamera.Position = Vector3.Back * distanceValue;
			xRotator.Rotation = Vector3.Right * Mathf.DegToRad(DefaultAngle);

			zoomDistanceCurveRanges = new(distanceCurve.MinDomain, distanceCurve.MaxDomain, distanceCurve.MinValue, distanceCurve.MaxValue);
			zoomSizeCurveRanges = new(sizeCurve.MinDomain, sizeCurve.MaxDomain, sizeCurve.MinValue, sizeCurve.MaxValue);

			zoomDistanceRange = MaxDistance - MinDistance;
			zoomSizeRange = MaxSize - MinSize;

			Zoom = zoomValue;
		}
	}


	public override void _EnterTree()
	{
		cameraTarget = new();
		yRotator = new();
		xRotator = new();
		virtualCamera = new();

		xRotator.AddChild(virtualCamera);
		yRotator.AddChild(xRotator);
		cameraTarget.AddChild(yRotator);
		AddChild(cameraTarget);

		cameraTarget.Owner = this;
		yRotator.Owner = this;
		xRotator.Owner = this;
		virtualCamera.Owner = this;
	}


	public override void _ExitTree()
	{
		xRotator.RemoveChild(virtualCamera);
		yRotator.RemoveChild(xRotator);
		cameraTarget.RemoveChild(yRotator);
		RemoveChild(cameraTarget);

		cameraTarget.QueueFree();
		yRotator.QueueFree();
		xRotator.QueueFree();
		virtualCamera.QueueFree();
	}


	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint() && IsInstanceValid(xRotator) && IsInstanceValid(virtualCamera))
		{
			virtualCamera.Position = Vector3.Back * distanceValue;
			xRotator.Rotation = Vector3.Right * Mathf.DegToRad(Mathf.Clamp(DefaultAngle, MinPitch, MaxPitch));
		}
	}


#region Public Methods


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


#endregion


#region Rotation


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


#endregion


#region Zoom


	private float GetSampledZoomDistance(float value) => Mathf.Remap(distanceCurve.Sample(Mathf.Remap(value, MinDistance, MaxDistance, zoomDistanceCurveRanges.X, zoomDistanceCurveRanges.Y)), zoomDistanceCurveRanges.Z, zoomDistanceCurveRanges.W, MinDistance, MaxDistance);
	private float GetSampledZoomSize(float value) => Mathf.Remap(sizeCurve.Sample(Mathf.Remap(value, MinSize, MaxSize, zoomSizeCurveRanges.X, zoomSizeCurveRanges.Y)), zoomSizeCurveRanges.Z, zoomSizeCurveRanges.W, MinSize, MaxSize);


	private void ZoomCamera()
	{
		switch (zoomBehavior)
		{
			case ZoomBehavior.Distance:
				ApplyDistanceZoom();
				break;

			case ZoomBehavior.Size:
				ApplySizeZoom();
				break;
		}
	}


	private void ApplyDistanceZoom()
	{
		DefaultDistance = GetSampledZoomDistance((zoomDistanceRange * Zoom) + MinDistance);

		if (IsInstanceValid(distanceTween) && distanceTween.IsRunning())
		{
			distanceTween.Kill();
		}

		distanceTween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
		distanceTween.TweenProperty(virtualCamera, positionProperty, Vector3.Back * DefaultDistance, distanceZoomDuration);
		distanceTween.Play();
	}


	private void ApplySizeZoom()
	{
		CameraResource.OrthographicSize = GetSampledZoomSize((zoomSizeRange * Zoom) + MinSize);
	}


#endregion


	private enum ZoomBehavior
	{
		Distance,
		Size,
	}
}
