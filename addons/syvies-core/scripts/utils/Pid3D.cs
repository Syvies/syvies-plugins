using Godot;

namespace SyviesCore.Utils;

public partial class Pid3D(float p, float i, float d, float saturation, float max, PidMeasurement measurement = PidMeasurement.Value) : RefCounted
{
	private float p = p, i = i, d = d;
	private PidMeasurement derivativeMeasurement = measurement;
	private float maxLength = max;
	private bool initialized = false;
	private Vector3 lastError = Vector3.Zero;
	private Vector3 lastValue = Vector3.Zero;
	private Vector3 integrationStored = Vector3.Zero;
	private float integralSaturation = saturation;
	// private Vector3 errorIntegral = Vector3.Zero;


	public Vector3 Update(Vector3 currentValue, Vector3 targetValue, float delta)
	{
		// errorIntegral += error * delta;
		// var derivative = (error - lastError) / delta;
		// lastError = error;
		// return p * error + i * errorIntegral + d * derivative;

		Vector3 error = targetValue - currentValue;

		// P term
		Vector3 proportional = error * p;

		// D term
		Vector3 derivativeMeasure;
		
		if (initialized)
		{
		
			Vector3 errorRateOfChange = (error - lastError) / delta;
			Vector3 valueRateOfChange = (currentValue - lastValue) / delta;

			derivativeMeasure = derivativeMeasurement switch
			{
				PidMeasurement.Error => errorRateOfChange,
				_ => -valueRateOfChange,
			};
		}
		else
		{
			derivativeMeasure = Vector3.Zero;
			initialized = true;
		}
		
		lastError = error;
		lastValue = currentValue;

		Vector3 derivative = derivativeMeasure * d;

		// I term
		integrationStored += error * delta;

		if (integrationStored.Length() > integralSaturation)
		{
			integrationStored = integrationStored.Normalized() * integralSaturation;
		}

		Vector3 integral = integrationStored * i;

		// Result
		Vector3 result = proportional + derivative + integral;

		if (result.Length() > maxLength)
		{
			result = result.Normalized() * maxLength;
		}

		return result;
	}


	public void Reset()
	{
		initialized = false;
	}
}


public enum PidMeasurement
{
	Value,
	Error,
}
