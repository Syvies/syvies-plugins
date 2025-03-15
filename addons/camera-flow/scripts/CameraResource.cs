using Godot;

namespace CameraFlow;

[Tool]
public partial class CameraResource : Resource
{
	[Signal] public delegate void PropertyUpdatedEventHandler();

	[Export] public Camera3D.KeepAspectEnum KeepAspect
	{
		get => keepAspectValue;
		set
		{
			if (keepAspectValue == value) { return; }

			keepAspectValue = value;
			EmitSignalPropertyUpdated();
		}
	}
	[Export(PropertyHint.Layers3DRender)] public uint CullMask
	{
		get => cullMaskValue;
		set
		{
			if (cullMaskValue == value) { return; }

			cullMaskValue = value;
			EmitSignalPropertyUpdated();
		}
	}
	[Export(PropertyHint.Range, "0.001,10,0.001,or_greater")] public float Near
	{
		get => nearValue;
		set
		{
			if (nearValue == value) { return; }

			nearValue = value;
			EmitSignalPropertyUpdated();
		}
	}
	[Export(PropertyHint.Range, "0.01,4000,0.001,or_greater")] public float Far
	{
		get => farValue;
		set
		{
			if (farValue == value) { return; }

			farValue = value;
			EmitSignalPropertyUpdated();
		}
	}
	[Export] public Camera3D.ProjectionType ProjectionType
	{
		get => projectionTypeValue;
		set
		{
			if (projectionTypeValue == value) { return; }

			projectionTypeValue = value;
			EmitSignalPropertyUpdated();
		}
	}
	[ExportGroup("Perspective")]
	[Export(PropertyHint.Range, "1,179,")] public float Fov
	{
		get => fovValue;
		set
		{
			if (fovValue == value) { return; }

			fovValue = value;
			EmitSignalPropertyUpdated();
		}
	}
	[ExportGroup("Orthographic")]
	[Export(PropertyHint.Range, "0.001,100,0.001,or_greater")] public float OrthographicSize
	{
		get => Size;
		set
		{
			if (Size == value) { return; }

			Size = value;
			EmitSignalPropertyUpdated();
		}
	}
	[ExportGroup("Frustum")]
	[Export(PropertyHint.Range, "0.001,100,0.001,or_greater")] public float FrustumSize
	{
		get => Size;
		set
		{
			if (Size == value) { return; }

			Size = value;
			EmitSignalPropertyUpdated();
		}
	}
	[Export] public Vector2 FrustumOffset
	{
		get => frustumOffsetValue;
		set
		{
			if (frustumOffsetValue == value) { return; }

			frustumOffsetValue = value;
			EmitSignalPropertyUpdated();
		}
	}

	public float Size { get; private set; } = 1f;

	private Camera3D.KeepAspectEnum keepAspectValue = Camera3D.KeepAspectEnum.Height;
	private uint cullMaskValue = uint.MaxValue;
	private float nearValue = 0.05f;
	private float farValue = 1000f;
	private Camera3D.ProjectionType projectionTypeValue = Camera3D.ProjectionType.Perspective;
	private float fovValue = 55f;
	private Vector2 frustumOffsetValue = Vector2.Zero;
}
