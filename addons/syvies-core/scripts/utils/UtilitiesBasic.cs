using Godot;

namespace SyviesCore.Utils;

public static class UtilitiesBasic
{
	public static Vector2 GetTransformAngleFromXZPlane(Transform3D transform)
	{
		Vector3 upVector = transform.Basis.Y.Normalized();
		Vector3 projectedVector = new Vector3(upVector.X, 0, upVector.Z).Normalized();
		float sign = Mathf.Sign(transform.Basis.Z.Y) < 0 ? -1f : 1f;
		float angle = Mathf.Acos(upVector.Dot(projectedVector));
		Vector2 inverseAngle = Vector2.FromAngle(angle);
		return new Vector2(inverseAngle.Y, inverseAngle.X * sign).Normalized();
	}
}