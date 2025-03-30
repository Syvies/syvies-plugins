using Godot;
using System;
using System.Threading.Tasks;
using SyviesCore.DebugUtils;
using SyviesCore.Utils;

namespace CameraFlow;

[Tool]
public partial class DynamicCamera : Camera3D
{
	private VirtualCamera currentCamera = null;
	private bool isInTransition = false;
	private Tween transitionTween = null;


	public override void _EnterTree()
	{
		CameraManager.CameraChanged += OnCameraChanged;
		OnCameraChanged(CameraManager.CurrentCamera);
	}


	public override void _ExitTree()
	{
		CameraManager.CameraChanged -= OnCameraChanged;
	}


	public override void _PhysicsProcess(double delta)
	{
		if (!IsInstanceValid(currentCamera) || isInTransition) { return; }

		GlobalTransform = currentCamera.GlobalTransform;
	}


	private void OnCameraChanged(VirtualCamera camera)
	{
		if (!IsInstanceValid(camera)) { return; }

		if (Engine.IsEditorHint())
		{
			ChangeCamera(camera);
		}
		else
		{
			ChangeCameraAsync(camera);
		}
	}


	private void ChangeCamera(VirtualCamera camera)
	{
		if (currentCamera == camera) { return; }

		if (IsInstanceValid(currentCamera) && !Engine.IsEditorHint())
		{
			currentCamera.CameraResource.Changed -= OnCameraResourceChanged;
		}

		currentCamera = camera;
		if (!IsInstanceValid(currentCamera)) { return; }

		CameraResource camResource = currentCamera.CameraResource;

		if (IsInstanceValid(camResource))
		{
			if (!Engine.IsEditorHint())
			{
				camResource.Changed += OnCameraResourceChanged;
			}
		}
		else {
			camResource = new();
		}

		GlobalPosition = currentCamera.GlobalPosition;
		Quaternion = currentCamera.Quaternion;
		Projection = camResource.ProjectionType;
		KeepAspect = camResource.KeepAspect;
		CullMask = camResource.CullMask;
		Near = camResource.Near;
		Far = camResource.Far;
		Fov = camResource.Fov;
		Size = camResource.Size;
		FrustumOffset = camResource.FrustumOffset;
	}

	private async void ChangeCameraAsync(VirtualCamera camera)
	{
		if (currentCamera == camera) { return; }

		await TransitionToCamera(currentCamera, camera);

		ChangeCamera(camera);
	}


	private async Task TransitionToCamera(VirtualCamera origin, VirtualCamera destination)
	{
		if (IsInstanceValid(transitionTween) && transitionTween.IsRunning())
		{
			transitionTween.Stop();
		}

		isInTransition = false;

		if (!IsInstanceValid(origin) || !IsInstanceValid(destination) || origin.CameraResource.ProjectionType != destination.CameraResource.ProjectionType) { return; }

		TweenResource camTween = null;

		if (destination.TransitionIn != null)
		{
			camTween = destination.TransitionIn;
		}
		else if (origin.TransitionOut != null)
		{
			camTween = origin.TransitionOut;
		}

		if (!IsInstanceValid(camTween)) { return; }
		
		isInTransition = true;

		transitionTween = GetTree().CreateTween().SetTrans(camTween.Transition).SetEase(camTween.Ease).SetParallel(true);

		transitionTween.TweenProperty(this, "global_position", destination.GlobalPosition, camTween.Duration);
		transitionTween.TweenProperty(this, "quaternion", destination.Quaternion, camTween.Duration);
		transitionTween.TweenProperty(this, "near", destination.CameraResource.Near, camTween.Duration);
		transitionTween.TweenProperty(this, "far", destination.CameraResource.Far, camTween.Duration);
		transitionTween.TweenProperty(this, "fov", destination.CameraResource.Fov, camTween.Duration);
		transitionTween.TweenProperty(this, "size", destination.CameraResource.Size, camTween.Duration);
		transitionTween.TweenProperty(this, "frustum_offset", destination.CameraResource.FrustumOffset, camTween.Duration);

		await ToSignal(transitionTween, Tween.SignalName.Finished);

		isInTransition = false;
	}


	private void OnCameraResourceChanged()
	{
		GlobalPosition = currentCamera.GlobalPosition;
		Quaternion = currentCamera.Quaternion;
		Projection = currentCamera.CameraResource.ProjectionType;
		KeepAspect = currentCamera.CameraResource.KeepAspect;
		CullMask = currentCamera.CameraResource.CullMask;
		Near = currentCamera.CameraResource.Near;
		Far = currentCamera.CameraResource.Far;
		Fov = currentCamera.CameraResource.Fov;
		Size = currentCamera.CameraResource.Size;
		FrustumOffset = currentCamera.CameraResource.FrustumOffset;
	}
}
