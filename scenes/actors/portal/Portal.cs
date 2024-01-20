using System;
using Godot;

public partial class Portal : Node3D
{
    [Export]
    private Camera3D _camera = null;

    [Export]
    private SubViewport _viewport = null;

    [Export]
    private CsgBox3D _visual = null;

    [Export]
    private Portal _exit = null;

    [Export]
    private int _cullLayer = 0;

    public int CullLayer
    {
        get { return _cullLayer; }
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _visual.SetLayerMaskValue(1, false);
        _visual.SetLayerMaskValue(_cullLayer, true);
        _camera.SetCullMaskValue(_exit.CullLayer, false);

        _camera.Environment = GetWorld3D().Environment;
        _camera.Environment.TonemapMode = Godot.Environment.ToneMapper.Linear;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        Camera3D mainCamera = GetViewport().GetCamera3D();

        Transform3D mainCameraRelPos = GlobalTransform.AffineInverse() * mainCamera.GlobalTransform;
        Transform3D targetTransform = _exit.GlobalTransform * mainCameraRelPos;

        _camera.GlobalTransform = targetTransform;

        _camera.CullMask = mainCamera.CullMask;
        _camera.SetCullMaskValue(_exit.CullLayer, false);

        _viewport.Size = (Vector2I)GetViewport().GetVisibleRect().Size;

        var windowAABB = _visual.GetAabb();
        var cornerOne = _exit.ToGlobal(
            new Vector3(windowAABB.Position.X, windowAABB.Position.Y, 0)
        );
        var cornerTwo = _exit.ToGlobal(
            new Vector3(windowAABB.Position.X + windowAABB.Size.X, windowAABB.Position.Y, 0)
        );
        var cornerThree = _exit.ToGlobal(
            new Vector3(
                windowAABB.Position.X + windowAABB.Size.X,
                windowAABB.Position.Y + windowAABB.Size.Y,
                0
            )
        );
        var cornerFour = _exit.ToGlobal(
            new Vector3(windowAABB.Position.X, windowAABB.Position.Y + windowAABB.Size.Y, 0)
        );

        var cameraFwd = -_camera.GlobalTransform.Basis.Z.Normalized();
        var a = (cornerOne - _camera.GlobalPosition).Dot(cameraFwd);
        var b = (cornerTwo - _camera.GlobalPosition).Dot(cameraFwd);
        var c = (cornerThree - _camera.GlobalPosition).Dot(cameraFwd);
        var d = (cornerFour - _camera.GlobalPosition).Dot(cameraFwd);

        _camera.Near = Mathf.Max(0.01f, Mathf.Min(Mathf.Min(Mathf.Min(a, b), c), d) - 0.05f);
        _camera.Far = GetViewport().GetCamera3D().Far;
        _camera.Fov = GetViewport().GetCamera3D().Fov;
        _camera.KeepAspect = GetViewport().GetCamera3D().KeepAspect;
    }
}
