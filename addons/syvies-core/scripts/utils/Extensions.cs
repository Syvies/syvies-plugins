using Godot;

namespace SyviesCore.Utils;

public static class Extensions
{
	public static bool IsNaN(this Vector2 vector)
	{
		return float.IsNaN(vector.X) && float.IsNaN(vector.Y);
	}
}