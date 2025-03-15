using Godot;

namespace SyviesCore.Utils;

public partial class RigidBody3DController : RigidBody3D
{
	private const float DEFAULT_MOVE_SPEED = 5f;
	private const float DEFAULT_ROTATION_SPEED = 5f;

	public bool IsMoving { get; private set; } = false;

	private Vector3 targetPosition = Vector3.Zero;
	private Quaternion targetRotation = Quaternion.Identity;
	private float moveSpeed = DEFAULT_MOVE_SPEED;
	private float rotationSpeed = DEFAULT_ROTATION_SPEED;


	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		if (!IsMoving) { return; }

		// Interpoler la position
		Vector3 newPosition = state.Transform.Origin.Lerp(targetPosition, moveSpeed * state.Step);

		// Interpoler la rotation
		Quaternion newRotation = state.Transform.Basis.GetRotationQuaternion().Slerp(targetRotation, rotationSpeed * state.Step);

		state.Transform = new Transform3D(new Basis(newRotation), newPosition);

		if (targetPosition.IsEqualApprox(newPosition) && targetRotation.IsEqualApprox(newRotation))
		{
			IsMoving = false;
		}
	}


	public void SetTargetPosition(Vector3 position, float speed = DEFAULT_MOVE_SPEED)
	{
		targetPosition = position;
		moveSpeed = speed;
		IsMoving = true;
	}


	public void SetTargetRotation(Quaternion rotation, float speed = DEFAULT_ROTATION_SPEED)
	{
		targetRotation = rotation;
		rotationSpeed = speed;
		IsMoving = true;
	}
}
