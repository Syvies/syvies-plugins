using Godot;

namespace SyviesCore.Utils;

public partial class NodeUtilities: Node
{
	private static NodeUtilities instance;


	public override void _EnterTree()
	{
		if (IsInstanceValid(instance) && instance != this)
		{
			GD.PushWarning("NodeUtilities instance already exists. Discarding new one.");
			GetParent().RemoveChild(this);
			QueueFree();
			return;
		}

		instance = this;
	}


	public override void _ExitTree()
	{
		if (IsInstanceValid(instance) && instance == this)
		{
			instance = null;
		}
	}


	public static bool TryGetTree(out SceneTree tree)
	{
		if (IsInstanceValid(instance))
		{
			tree = instance.GetTree();
			return IsInstanceValid(tree);
		}

		tree = null;
		return false;
	}


	public static bool TryGetCamera3D(out Camera3D camera3D)
	{
		if (IsInstanceValid(instance))
		{
			camera3D = instance.GetViewport().GetCamera3D();
			return IsInstanceValid(camera3D);
		}

		camera3D = null;
		return false;
}
}