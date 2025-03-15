using Godot;
using Godot.Collections;

namespace SyviesCore.Utils;

public partial class Utilities3D : Node3D
{
	private static Utilities3D instance;


	public override void _EnterTree()
	{
		if (IsInstanceValid(instance) && instance != this)
		{
			GD.PushWarning("Utilities3D instance already exists. Discarding new one.");
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


	public static Camera3D GetCamera3D() => instance.GetViewport().GetCamera3D();


	public static Dictionary RayCast(Vector3 from, Vector3 to, uint layer = uint.MaxValue, bool collideWithBodies = true, bool collideWithAreas = false)
	{
		if (!IsInstanceValid(instance) || from.IsEqualApprox(to)) { return null; }

		PhysicsRayQueryParameters3D query = new(){
			From = from,
			To = to,
			CollideWithBodies = collideWithBodies,
			CollideWithAreas = collideWithAreas,
			CollisionMask = layer
		};

		return instance.GetWorld3D().DirectSpaceState.IntersectRay(query);
	}
}