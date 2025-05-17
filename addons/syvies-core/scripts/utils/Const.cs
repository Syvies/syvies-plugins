using Godot;

namespace SyviesCore.Utils;

public static class Const
{
	public const float F_ZERO = 0f;
	public const int I_ZERO = 0;

	public static readonly Vector2 Vec2NaN = new(float.NaN, float.NaN);
	public static readonly Vector3 Vec3NaN = new(float.NaN, float.NaN, float.NaN);
}