using Godot;
using SyviesCore.DebugUtils;

namespace SyviesCore.StateMachine;

public partial class StateMachine : Node
{
	[Signal] public delegate void TransitionedEventHandler(StringName stateName);

	public static readonly StringName stateMachineGroup = new("state_machine");

	[Export] private NodePath initialState;

	public Node StateOwner { get; private set; }

	private State currentState = null;
	private StringName debugName = null;


	public StateMachine()
	{
		AddToGroup(stateMachineGroup);
	}


	public override void _Ready()
	{
		StateOwner = GetParent();
		debugName = new StringName($"{StateOwner.Name} State Machine".Capitalize());

		TransitionToInitialState();
	}


	public override void _Process(double delta)
	{
		currentState?.Update(delta);
	}


	public override void _PhysicsProcess(double delta)
	{
		currentState?.PhysicsUpdate(delta);
		AddDebugProperties();
	}


	public override void _Input(InputEvent @event)
	{
		currentState?.Input(@event);
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		currentState?.UnhandledInput(@event);
	}


	public void TransitionTo(NodePath statePath)
	{
		if (HasNode(statePath) == false) {return;}

		Node stateNode = GetNode(statePath);

		if (stateNode != null && stateNode is State newState && newState != currentState)
		{
			currentState?.Exit();

			currentState = newState;
			currentState.Enter();

			EmitSignalTransitioned(currentState.Name);
		}
	}


	public StringName GetCurrentStateName()
	{
		if (currentState == null)
		{
			return null;
		}
		return currentState.Name;
	}


	private void AddDebugProperties()
	{
		if (!OS.IsDebugBuild() || debugName == null)
		{
			return;
		}

		if (currentState != null)
		{
			DebugPanel.AddDebugProperty(debugName, currentState.Name);
		}
		else
		{
			DebugPanel.AddDebugProperty(debugName, "<null>");
		}
	}


	private async void TransitionToInitialState()
	{
		if (StateOwner.IsNodeReady() == false)
		{
			await ToSignal(StateOwner, Node.SignalName.Ready);
		}
		TransitionTo(initialState);
	}
}
