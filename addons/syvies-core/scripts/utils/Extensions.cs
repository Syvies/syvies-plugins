using Godot;

namespace SyviesCore.Utils;

public static class Extensions
{
	public static bool IsNaN(this Vector2 vector)
	{
		return float.IsNaN(vector.X) && float.IsNaN(vector.Y);
	}


	public static bool IsNaN(this Vector3 vector)
	{
		return float.IsNaN(vector.X) && float.IsNaN(vector.Y) && float.IsNaN(vector.Z);
	}


	public static Transform3D NormalAligned(this Transform3D transform, Vector3 normal)
	{
		if (normal.IsZeroApprox()) { return transform; }

		normal = normal.Normalized();

		Basis aligned = transform.Basis;
		aligned.Y = normal;
		aligned.X = -aligned.Z.Cross(normal);

		return new Transform3D(aligned, transform.Origin).Orthonormalized();
	}
}