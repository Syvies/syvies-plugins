using Godot;

namespace SyviesCore.Utils;

public partial class Pid3D(float p, float i, float d) : RefCounted
{
	private float p = p, i = i, d = d;
	private Vector3 lastError;
	private Vector3 errorIntegral;


	public Vector3 Update(Vector3 error, float delta)
	{
		errorIntegral += error * delta;
		var derivative = (error - lastError) / delta;
		lastError = error;
		return p * error + i * errorIntegral + d * derivative;
	}
}
