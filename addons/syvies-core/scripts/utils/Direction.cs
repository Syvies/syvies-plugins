using Godot;

namespace SyviesCore.Utils;

public static class DirectionExtension
{
	public static Vector3I ToVector3I(this Direction direction)
	{
		return direction switch
		{
			Direction.North => Vector3I.Forward,
			Direction.South => Vector3I.Back,
			Direction.East => Vector3I.Right,
			Direction.West => Vector3I.Left,
			Direction.Up => Vector3I.Up,
			Direction.Down => Vector3I.Down,
			_ => Vector3I.Zero,
		};
	}

	
}


public enum Direction
{
	North,
	South,
	East,
	West,
	Up,
	Down,
}
