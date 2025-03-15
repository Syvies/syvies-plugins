using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace CameraFlow;

[Tool]
public partial class CameraManager : Node
{
	private static CameraManager instance;
	public static Action<VirtualCamera> CameraChanged;
	public static VirtualCamera CurrentCamera { get; private set;}

	private readonly List<VirtualCamera> virtualCameras = [];


	public override void _EnterTree()
	{
		if (IsInstanceValid(instance) && instance != this)
		{
			GD.PushWarning("CameraManager instance already exists. Discarding new one.");
			GetParent().RemoveChild(this);
			QueueFree();
			return;
		}

		instance = this;
	}


	public override void _ExitTree()
	{
		if (instance == this)
		{
			instance = null;
			CurrentCamera = null;
		}

		virtualCameras.Clear();
	}


	public override void _Ready()
	{
		GetAllVirtualCameras();
		OrderVirtualCameras();
		GetCurrentCamera();
	}


#region Virtual Cameras


	public static bool AddVirtualCamera(VirtualCamera virtualCamera)
	{
		if (IsInstanceValid(instance) && IsInstanceValid(virtualCamera) && !instance.virtualCameras.Contains(virtualCamera))
		{
			instance.virtualCameras.Add(virtualCamera);
			instance.OrderVirtualCameras();
			instance.GetCurrentCamera();
			return true;
		}
		return false;
	}


	public static void RemoveVirtualCamera(VirtualCamera virtualCamera)
	{
		if (!IsInstanceValid(instance)) { return; }

		instance.virtualCameras.Remove(virtualCamera);
		instance.GetCurrentCamera();
	}


	public static void PriorityUpdated()
	{
		if (!IsInstanceValid(instance)) { return; }

		instance.OrderVirtualCameras();
		instance.GetCurrentCamera();
	}


#endregion


#region Utilities


	private void GetAllVirtualCameras()
	{
		virtualCameras.Clear();
		Array<Node> cameraNodes = GetTree().GetNodesInGroup(VirtualCamera.virtualCameraGroup);

		foreach (Node cameraNode in cameraNodes)
		{
			if (cameraNode is VirtualCamera virtualCamera && !virtualCameras.Contains(virtualCamera))
			{
				virtualCameras.Add(virtualCamera);
			}
		}
	}


	private void OrderVirtualCameras()
	{
		virtualCameras.Sort((a, b) => a.Priority.CompareTo(b.Priority));
	}


	private void GetCurrentCamera()
	{
		int lastIndex = virtualCameras.Count - 1;
		VirtualCamera newCamera = null;

		if (lastIndex >= 0)
		{
			newCamera = virtualCameras[lastIndex];
		}
		
		if (CurrentCamera != newCamera)
		{
			CurrentCamera = newCamera;
			CameraChanged?.Invoke(CurrentCamera);
		}
	}


#endregion

}


public enum CameraPriority
{
	Disabled,
	Player,
	Cinematic,
	MaxPriority,
}
