using Godot;
using SyviesCore.DebugUtils;

namespace CameraFlow;

[Tool]
public partial class CameraResource : Resource
{
	[Export] public Camera3D.KeepAspectEnum KeepAspect
	{
		get => keepAspectValue;
		set
		{
			if (keepAspectValue == value) { return; }

			keepAspectValue = value;
			EmitChanged();
		}
	}
	[Export(PropertyHint.Layers3DRender)] public uint CullMask
	{
		get => cullMaskValue;
		set
		{
			if (cullMaskValue == value) { return; }

			cullMaskValue = value;
			EmitChanged();
		}
	}
	[Export(PropertyHint.Range, "0.001,10,,or_greater")] public float Near
	{
		get => nearValue;
		set
		{
			if (nearValue == value) { return; }

			nearValue = value;
			EmitChanged();
		}
	}
	[Export(PropertyHint.Range, "0.01,4000,,or_greater")] public float Far
	{
		get => farValue;
		set
		{
			if (farValue == value) { return; }

			farValue = value;
			EmitChanged();
		}
	}
	[Export] public Camera3D.ProjectionType ProjectionType
	{
		get => projectionTypeValue;
		set
		{
			if (projectionTypeValue == value) { return; }

			projectionTypeValue = value;
			EmitChanged();
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
			EmitChanged();
			Logger.Info($"FOV: {fovValue}");
		}
	}
	[ExportGroup("Orthographic")]
	[Export(PropertyHint.Range, "0.001,100,,or_greater")] public float OrthographicSize
	{
		get => Size;
		set
		{
			if (Size == value) { return; }

			Size = value;
			EmitChanged();
		}
	}
	[ExportGroup("Frustum")]
	[Export(PropertyHint.Range, "0.001,100,,or_greater")] public float FrustumSize
	{
		get => Size;
		set
		{
			if (Size == value) { return; }

			Size = value;
			EmitChanged();
		}
	}
	[Export] public Vector2 FrustumOffset
	{
		get => frustumOffsetValue;
		set
		{
			if (frustumOffsetValue == value) { return; }

			frustumOffsetValue = value;
			EmitChanged();
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
