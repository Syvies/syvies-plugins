using Godot;

namespace CameraFlow;

public partial class VirtualCameraGizmo : EditorNode3DGizmoPlugin
{
	private const string ICON_PATH = "res://addons/camera-flow/icons/VirtualCameraGizmo.png";
	private const string ICON_MATERIAL = "icon_material";
	private const string LINES_MATERIAL = "lines_material";
	private const float ICON_SIZE = 0.05f;

	private static readonly StringName viewportWidthSetting = new("display/window/size/viewport_width");
	private static readonly StringName viewportHeightSetting = new("display/window/size/viewport_height");
	private static readonly Texture2D gizmoIcon = GD.Load<Texture2D>(ICON_PATH);
	private static readonly Color gizmoColor = Colors.CornflowerBlue;


	public VirtualCameraGizmo()
	{
		CreateIconMaterial(ICON_MATERIAL, gizmoIcon, false, gizmoColor);
		CreateMaterial(LINES_MATERIAL, gizmoColor, false, false, true);
	}


	public override string _GetGizmoName()
	{
		return PluginCameraFlow.GIZMO_NAME;
	}


	public override bool _HasGizmo(Node3D forNode3D)
	{
		return forNode3D is VirtualCamera;
	}


	public override void _Redraw(EditorNode3DGizmo gizmo)
	{
		gizmo.Clear();

		VirtualCamera virtualCamera = (VirtualCamera)gizmo.GetNode3D();
		
		Vector2 viewportSize = new(ProjectSettings.GetSetting(viewportWidthSetting, 1920f).AsSingle(), ProjectSettings.GetSetting(viewportHeightSetting, 1080f).AsSingle());
		float viewportAspect = viewportSize.X > 0f && viewportSize.Y > 0f ? viewportSize.X / viewportSize.Y : 1f;
		Vector2 sizeFactor = viewportAspect > 1f ? new Vector2(1f, 1f / viewportAspect) : new Vector2(viewportAspect, 1f);

		Vector3[] linesVertices = [];

		switch (virtualCamera.CameraResource.ProjectionType)
		{
			case Camera3D.ProjectionType.Perspective:
				GetPerspectiveLines(ref linesVertices, virtualCamera, sizeFactor);
				break;

			case Camera3D.ProjectionType.Orthogonal:
				GetOrthogonalLines(ref linesVertices, virtualCamera, viewportAspect);
				break;

			case Camera3D.ProjectionType.Frustum:
				GetFrustumLines(ref linesVertices, virtualCamera, sizeFactor);
				break;
		}

		gizmo.AddUnscaledBillboard(GetMaterial(ICON_MATERIAL, gizmo), ICON_SIZE);
		gizmo.AddLines(linesVertices, GetMaterial(LINES_MATERIAL, gizmo));
	}


	private static void GetPerspectiveLines(ref Vector3[] linesVertices, VirtualCamera virtualCamera, Vector2 sizeFactor)
	{
		float fov = virtualCamera.CameraResource.Fov / 2f;
		float hSize = Mathf.Sin(Mathf.DegToRad(fov));
		float depth = -Mathf.Cos(Mathf.DegToRad(fov));

		Vector3 side;
		if (virtualCamera.CameraResource.KeepAspect == Camera3D.KeepAspectEnum.Width)
		{
			side =  new(hSize * sizeFactor.X, 0f, depth * sizeFactor.X);
		}
		else
		{
			side =  new(hSize * sizeFactor.X, 0f, depth * sizeFactor.Y);
		}

		Vector3 nSide = new(-side.X, side.Y, side.Z);
		Vector3 up = new(0f, hSize * sizeFactor.Y, 0f);

		AddTriangleLines(ref linesVertices, Vector3.Zero, side + up, side - up);
		AddTriangleLines(ref linesVertices, Vector3.Zero, nSide + up, nSide - up);
		AddTriangleLines(ref linesVertices, Vector3.Zero, side + up, nSide + up);
		AddTriangleLines(ref linesVertices, Vector3.Zero, side - up, nSide - up);

		side.X = Mathf.Min(side.X, hSize * 0.25f);
		nSide.X = -side.X;
		Vector3 tUp = new(0f, up.Y + hSize / 2f, side.Z);
		AddTriangleLines(ref linesVertices, tUp, side + up, nSide + up);
	}


	private static void GetOrthogonalLines(ref Vector3[] linesVertices, VirtualCamera virtualCamera, float viewportAspect)
	{
		float size = virtualCamera.CameraResource.OrthographicSize;
		float keepSize = size / 2f;

		Vector3 right, up;
		if (virtualCamera.CameraResource.KeepAspect == Camera3D.KeepAspectEnum.Width)
		{
			right = new(keepSize, 0f, 0f);
			up = new(0f, keepSize / viewportAspect, 0f);
		}
		else
		{
			right = new(keepSize * viewportAspect, 0f, 0f);
			up = new(0f, keepSize, 0f);
		}

		Vector3 back = new(0f, 0f, -1f);

		AddQuadLines(ref linesVertices, -up - right, -up + right, up + right, up - right);
		AddQuadLines(ref linesVertices, -up - right + back, -up + right + back, up + right + back, up - right + back);
		AddQuadLines(ref linesVertices, up + right, up + right + back, up - right + back, up - right);
		AddQuadLines(ref linesVertices, -up + right, -up + right + back, -up - right + back, -up - right);

		right.X = Mathf.Min(right.X, keepSize * 0.25f);
		Vector3 tUp = new(0f, up.Y + keepSize / 2f, back.Z);
		AddTriangleLines(ref linesVertices, tUp, right + up + back, -right + up + back);
	}


	private static void GetFrustumLines(ref Vector3[] linesVertices, VirtualCamera virtualCamera, Vector2 sizeFactor)
	{
		float hSize = virtualCamera.CameraResource.FrustumSize / 2f;

		Vector3 side = new Vector3(hSize, 0f, -virtualCamera.CameraResource.Near).Normalized();
		side.X *= sizeFactor.X;
		Vector3 nSide = new(-side.X, side.Y, side.Z);
		Vector3 up = new(0f, hSize * sizeFactor.Y, 0f);
		Vector3 offset = new(virtualCamera.CameraResource.FrustumOffset.X, virtualCamera.CameraResource.FrustumOffset.Y, 0f);

		AddTriangleLines(ref linesVertices, Vector3.Zero, side + up + offset, side - up + offset);
		AddTriangleLines(ref linesVertices, Vector3.Zero, nSide + up + offset, nSide - up + offset);
		AddTriangleLines(ref linesVertices, Vector3.Zero, side + up + offset, nSide + up + offset);
		AddTriangleLines(ref linesVertices, Vector3.Zero, side - up + offset, nSide - up + offset);

		side.X = Mathf.Min(side.X, hSize * 0.25f);
		nSide.X = -side.X;
		Vector3 tUp = new(0f, up.Y + hSize / 2f, side.Z);
		AddTriangleLines(ref linesVertices, tUp + offset, side + up + offset, nSide + up + offset);
	}


	private static void AddTriangleLines(ref Vector3[] vertices, Vector3 pointA, Vector3 pointB, Vector3 pointC)
	{
		vertices = [.. vertices, pointA, pointB, pointB, pointC, pointC, pointA];
	}


	private static void AddQuadLines(ref Vector3[] vertices, Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD)
	{
		vertices = [.. vertices, pointA, pointB, pointB, pointC, pointC, pointD, pointD, pointA];
	}
}
