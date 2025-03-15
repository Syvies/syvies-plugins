#if TOOLS
using Godot;

namespace SyviesCore;

[Tool]
public partial class PluginCore : EditorPlugin
{
	// --- Autoload ---
	private const string AUTOLOAD_NAME = "SyviesCore";
	private const string AUTOLOAD_PATH = "res://addons/syvies-core/scenes/core_autoload.tscn";
	// --- Node Inheritance ---
	private const string REF_COUNTED_TYPE = "RefCounted";
	private const string NODE_TYPE = "Node";
	private const string RIGID_BODY_3D_TYPE = "RigidBody3D";
	// --- State Machine ---
	private const string STATE_NAME = "State";
	private const string STATE_SCRIPT = "res://addons/syvies-core/scripts/state-machine/State.cs";
	private const string STATE_ICON = "res://addons/syvies-core/icons/State.png";
	private const string STATE_MACHINE_NAME = "StateMachine";
	private const string STATE_MACHINE_SCRIPT = "res://addons/syvies-core/scripts/state-machine/StateMachine.cs";
	private const string STATE_MACHINE_ICON = "res://addons/syvies-core/icons/StateMachine.png";
	// --- PID ---
	private const string PID_3D_NAME = "Pid3D";
	private const string PID_3D_SCRIPT = "res://addons/syvies-core/scripts/utils/Pid3D.cs";
	private const string PID_3D_ICON = "3D";
	// --- RigidBody3DController ---
	private const string RB3D_CONTROLLER_NAME = "RigidBody3DController";
	private const string RB3D_CONTROLLER_SCRIPT = "res://addons/syvies-core/scripts/utils/RigidBody3DController.cs";
	private const string RB3D_CONTROLLER_ICON = "res://addons/syvies-core/icons/RigidBody3DController.png";



	public override void _EnterTree()
	{
		Control gui = EditorInterface.Singleton.GetBaseControl();

		// --- State Machine ---
		Script stateScript = GD.Load<Script>(STATE_SCRIPT);
		Texture2D stateIcon = GD.Load<Texture2D>(STATE_ICON);
		AddCustomType(STATE_NAME, NODE_TYPE, stateScript, stateIcon);

		Script stateMachineScript = GD.Load<Script>(STATE_MACHINE_SCRIPT);
		Texture2D stateMachineIcon = GD.Load<Texture2D>(STATE_MACHINE_ICON);
		AddCustomType(STATE_MACHINE_NAME, NODE_TYPE, stateMachineScript, stateMachineIcon);

		// --- PID ---
		Script pid3DScript = GD.Load<Script>(PID_3D_SCRIPT);
		Texture2D pid3DIcon = gui.GetThemeIcon(PID_3D_ICON, "EditorIcons");
		AddCustomType(PID_3D_NAME, REF_COUNTED_TYPE, pid3DScript, pid3DIcon);

		// --- RigidBody3DController ---
		Script rb3DControllerScript = GD.Load<Script>(RB3D_CONTROLLER_SCRIPT);
		Texture2D rb3DControllerIcon = GD.Load<Texture2D>(RB3D_CONTROLLER_ICON);
		AddCustomType(RB3D_CONTROLLER_NAME, RIGID_BODY_3D_TYPE, rb3DControllerScript, rb3DControllerIcon);
	}


	public override void _ExitTree()
	{
		RemoveCustomType(STATE_NAME);
		RemoveCustomType(STATE_MACHINE_NAME);
		RemoveCustomType(PID_3D_NAME);
		RemoveCustomType(RB3D_CONTROLLER_NAME);
	}


	public override void _EnablePlugin()
	{
		AddAutoloadSingleton(AUTOLOAD_NAME, AUTOLOAD_PATH);
	}


	public override void _DisablePlugin()
	{
		RemoveAutoloadSingleton(AUTOLOAD_NAME);
	}
}
#endif
