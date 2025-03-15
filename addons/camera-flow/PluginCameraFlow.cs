#if TOOLS
using Godot;

namespace CameraFlow;

[Tool]
public partial class PluginCameraFlow : EditorPlugin
{
	public const string PLUGIN_FOLDER = "camera-flow";
	public const string GIZMO_NAME = "CameraFlow";
	private const string CORE_PLUGIN_FOLDER = "syvies-core";
	// --- Autoload ---
	private const string AUTOLOAD_NAME = "CameraFlow";
	private const string AUTOLOAD_PATH = "res://addons/camera-flow/scenes/camera_flow_autoload.tscn";
	// --- Node Inheritance ---
	private const string RESOURCE = "Resource";
	private const string NODE_3D = "Node3D";
	private const string CAMERA_3D = "Camera3D";
	private const string MARKER_3D = "Marker3D";
	// --- Custom Nodes ---
	private const string CAMERA_RESOURCE_NAME = "CameraResource";
	private const string CAMERA_RESOURCE_SCRIPT = "res://addons/camera-flow/scripts/CameraResource.cs";
	private const string CAMERA_RESOURCE_ICON = "res://addons/camera-flow/icons/CameraResource.png";
	private const string VIRTUAL_CAMERA_NAME = "VirtualCamera";
	private const string VIRTUAL_CAMERA_SCRIPT = "res://addons/camera-flow/scripts/VirtualCamera.cs";
	private const string VIRTUAL_CAMERA_ICON = "res://addons/camera-flow/icons/VirtualCamera.png";
	private const string DYNAMIC_CAMERA_NAME = "DynamicCamera";
	private const string DYNAMIC_CAMERA_SCRIPT = "res://addons/camera-flow/scripts/DynamicCamera.cs";
	private const string DYNAMIC_CAMERA_ICON = "res://addons/camera-flow/icons/DynamicCamera.png";
	private const string THIRD_PERSON_NAME = "ThirdPersonCamera";
	private const string THIRD_PERSON_SCRIPT = "res://addons/camera-flow/scripts/ThirdPersonCamera.cs";
	private const string THIRD_PERSON_ICON = "res://addons/camera-flow/icons/ThirdPersonCamera.png";

	VirtualCameraGizmo virtualCameraGizmo = new();


	public override void _EnterTree()
	{
		Script cameraResourceScript = GD.Load<Script>(CAMERA_RESOURCE_SCRIPT);
		Texture2D cameraResourceIcon = GD.Load<Texture2D>(CAMERA_RESOURCE_ICON);
		AddCustomType(CAMERA_RESOURCE_NAME, RESOURCE, cameraResourceScript, cameraResourceIcon);

		Script virtualCamScript = GD.Load<Script>(VIRTUAL_CAMERA_SCRIPT);
		Texture2D virtualCamIcon = GD.Load<Texture2D>(VIRTUAL_CAMERA_ICON);
		AddCustomType(VIRTUAL_CAMERA_NAME, NODE_3D, virtualCamScript, virtualCamIcon);

		Script dynamicCamScript = GD.Load<Script>(DYNAMIC_CAMERA_SCRIPT);
		Texture2D dynamicCamIcon = GD.Load<Texture2D>(DYNAMIC_CAMERA_ICON);
		AddCustomType(DYNAMIC_CAMERA_NAME, CAMERA_3D, dynamicCamScript, dynamicCamIcon);

		Script thirdPersonScript = GD.Load<Script>(THIRD_PERSON_SCRIPT);
		Texture2D thirdPersonIcon = GD.Load<Texture2D>(THIRD_PERSON_ICON);
		AddCustomType(THIRD_PERSON_NAME, MARKER_3D, thirdPersonScript, thirdPersonIcon);

		AddNode3DGizmoPlugin(virtualCameraGizmo);
	}


	public override void _ExitTree()
	{
		RemoveNode3DGizmoPlugin(virtualCameraGizmo);

		RemoveCustomType(VIRTUAL_CAMERA_NAME);
		RemoveCustomType(DYNAMIC_CAMERA_NAME);
		RemoveCustomType(THIRD_PERSON_NAME);
	}


	public override void _EnablePlugin()
	{
		AddAutoloadSingleton(AUTOLOAD_NAME, AUTOLOAD_PATH);

		if (!CheckForPluginCore())
		{
			GD.PushError($"{PLUGIN_FOLDER} requires {CORE_PLUGIN_FOLDER} to work.");
			EditorInterface.Singleton.SetPluginEnabled(PLUGIN_FOLDER, false);
		}
	}


	public override void _DisablePlugin()
	{
		RemoveAutoloadSingleton(AUTOLOAD_NAME);
	}


	private static bool CheckForPluginCore()
	{
		if (EditorInterface.Singleton.IsPluginEnabled(CORE_PLUGIN_FOLDER)) { return true; }

		GD.PushWarning($"Trying to enable {CORE_PLUGIN_FOLDER}.");
		EditorInterface.Singleton.SetPluginEnabled(CORE_PLUGIN_FOLDER, true);

		return EditorInterface.Singleton.IsPluginEnabled(CORE_PLUGIN_FOLDER);
	}
}
#endif
