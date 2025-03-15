using Godot;
using Godot.Collections;

namespace SyviesCore.Utils;

public static class MouseUtilities
{
	private const float DEGREES_PER_MOUSE_UNIT = -0.001f;
	private const float DEFAULT_RAY_LENGTH = 1000f;

	public static Vector2 GetMouseMotion(InputEventMouseMotion mouseMotion)
	{
		if (NodeUtilities.TryGetTree(out SceneTree tree))
		{
			Transform2D viewportTransform = tree.Root.GetFinalTransform();
			InputEvent relativeEvent = mouseMotion.XformedBy(viewportTransform);
			InputEventMouseMotion relativeMouseMotion = (InputEventMouseMotion)relativeEvent;
			Vector2 motion = relativeMouseMotion.Relative;

			motion *= DEGREES_PER_MOUSE_UNIT;
			//TODO - Add player settings for mouse sensitivity
			motion *= 50f;

			if (!motion.IsZeroApprox())
			{
				return motion;
			}
		}

		return Vector2.Zero;
	}


	public static Dictionary GetMouseRayCast(float rayLength = DEFAULT_RAY_LENGTH, uint layer = uint.MaxValue, bool collideWithBodies = true, bool collideWithAreas = false)
	{
		Dictionary result = null;

		if (NodeUtilities.TryGetCamera3D(out Camera3D camera))
		{
			Vector2 mousePosition = camera.GetViewport().GetMousePosition();
			Vector3 from = camera.ProjectRayOrigin(mousePosition);
			Vector3 to = from + (camera.ProjectRayNormal(mousePosition) * rayLength);

			result = Utilities3D.RayCast(from, to, layer, collideWithBodies, collideWithAreas);
		}

		return result;
	}


	public static Vector3 GetMouseWorldPosition(float rayLength = DEFAULT_RAY_LENGTH, uint layer = uint.MaxValue, bool collideWithBodies = true, bool collideWithAreas = false)
	{
		Vector3 position = Vector3.Zero;

		Dictionary rayResult = GetMouseRayCast(rayLength, layer, collideWithBodies, collideWithAreas);

		if (rayResult.Count > 0)
		{
			if (rayResult.TryGetValue("position", out Variant positionVariant))
			{
				position = positionVariant.AsVector3();
			}
		}

		return position;
	}
}