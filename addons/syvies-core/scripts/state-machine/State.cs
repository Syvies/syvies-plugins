using Godot;

namespace SyviesCore.StateMachine;

public partial class State : Node
{
	public static readonly StringName stateGroup = new("state");

	public StateMachine StateMachine { get; private set; }
	public Node ParentNode { get; private set; }


	public State()
	{
		AddToGroup(stateGroup);
	}


	public override void _Ready()
	{
		SetPhysicsProcess(false);
		SetProcess(false);
		SetProcessInternal(false);
		SetProcessShortcutInput(false);
		SetProcessUnhandledInput(false);
		SetProcessUnhandledKeyInput(false);

		StateMachine = GetStateMachine(this);
		ParentNode = GetParent();
	}


	public virtual void Enter() { }


	public virtual void Exit() { }


	public virtual void Update(double delta)
	{
		if (ParentNode is State parentState)
		{
			parentState.Update(delta);
		}
	}


	public virtual void PhysicsUpdate(double delta)
	{
		if (ParentNode is State parentState)
		{
			parentState.PhysicsUpdate(delta);
		}
	}


	public virtual void Input(InputEvent @event)
	{
		if (ParentNode is State parentState)
		{
			parentState.Input(@event);
		}
	}


	public virtual void UnhandledInput(InputEvent @event)
	{
		if (ParentNode is State parentState)
		{
			parentState.UnhandledInput(@event);
		}
	}


	private StateMachine GetStateMachine(Node node)
	{
		if (node == null)
		{
			return null;
		}
		else if (node is StateMachine)
		{
			return (StateMachine)node;
		}
		return GetStateMachine(node.GetParent());
	}
}
