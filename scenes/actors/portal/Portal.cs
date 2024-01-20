using System;
using Godot;

public partial class Portal : Area3D
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

    public CsgBox3D Visual
    {
        get { return _visual; }
    }

    private Node3D _player = null;
    private Vector3 _lastPos = Vector3.Zero;

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
        DoThing();
    }

    private void FixedUpdate()
    {
        DoThing();
    }

    public void DoThing()
    {
        if (_player != null)
        {
            Player player = (Player)_player;
            Vector3 offset = player.Camera.GlobalPosition - GlobalPosition;
            Vector3 prevOffset = _lastPos - GlobalPosition;

            int side = NonZeroSign(offset.Dot(GlobalTransform.Basis.Z));
            int prevSide = NonZeroSign(prevOffset.Dot(GlobalTransform.Basis.Z));

            if (side != prevSide)
            {
                GD.Print("We moved through");

                Transform3D foo = GlobalTransform.AffineInverse() * _player.GlobalTransform;
                Transform3D targetTransform = _exit.GlobalTransform * foo;

                _player.GlobalTransform = targetTransform;

                var r = _exit.GlobalTransform.Basis.GetEuler() - GlobalTransform.Basis.GetEuler();
                player.Velocity = player
                    .Velocity.Rotated(new Vector3(1, 0, 0), r.X)
                    .Rotated(new Vector3(0, 1, 0), r.Y)
                    .Rotated(new Vector3(0, 0, 1), r.Z);

                if (!player.UpDirection.IsEqualApprox(player.GlobalTransform.Basis.Y))
                {
                    player.UpDirection = player.GlobalTransform.Basis.Y;
                    player.Velocity -= player.Velocity * player.UpDirection.Normalized();
                }
                _exit.DoThing();
            }

            _lastPos = _player.GlobalPosition;
        }

        MoveCamera();
        Thicken();
    }

    private void MoveCamera()
    {
        Camera3D mainCamera = GetViewport().GetCamera3D();

        Transform3D mainCameraRelPos = GlobalTransform.AffineInverse() * mainCamera.GlobalTransform;
        Transform3D targetTransform = _exit.GlobalTransform * mainCameraRelPos;

        _camera.GlobalTransform = targetTransform;
        _camera.Fov = mainCamera.Fov;

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

        _camera.Near = Mathf.Max(0.01f, Mathf.Min(Mathf.Min(Mathf.Min(a, b), c), d) - 0.15f);
        _camera.Far = GetViewport().GetCamera3D().Far;
        _camera.Fov = GetViewport().GetCamera3D().Fov;
        _camera.KeepAspect = GetViewport().GetCamera3D().KeepAspect;
    }

    private void Thicken()
    {
        var camera = GetViewport().GetCamera3D();
        var forward = GlobalTransform.Basis.Z;
        var right = GlobalTransform.Basis.X;
        var up = GlobalTransform.Basis.Y;

        var cameraOffset = camera.GlobalPosition - GlobalPosition;
        var distFromPortalFwd = cameraOffset.Dot(forward);
        var distFromPortalRight = cameraOffset.Dot(right);
        var distFromPortalUp = cameraOffset.Dot(up);
        var portalSide = NonZeroSign(distFromPortalFwd);

        var halfWidth = _visual.Size.X / 2.0f;
        var halfHeight = _visual.Size.Y / 2.0f;

        if (
            Mathf.Abs(distFromPortalFwd) > 1.0f
            || Mathf.Abs(distFromPortalRight) > halfWidth + 0.3f
            || distFromPortalUp > halfHeight + 0.3f
        )
        {
            _visual.Size = new Vector3(_visual.Size.X, _visual.Size.Y, 0.0f);
            _visual.Position = new Vector3(_visual.Position.X, _visual.Position.Y, 0.0f);

            return;
        }

        var thickness = 0.3f;
        _visual.Size = new Vector3(_visual.Size.X, _visual.Size.Y, thickness);
        if (portalSide == 1)
        {
            _visual.Position = new Vector3(
                _visual.Position.X,
                _visual.Position.Y,
                -thickness / 2.0f
            );
        }
        else
        {
            _visual.Position = new Vector3(
                _visual.Position.X,
                _visual.Position.Y,
                thickness / 2.0f
            );
        }
    }

    public void _on_body_entered(PhysicsBody3D body)
    {
        GD.Print("Player entered");
        _player = body;
        Player player = (Player)_player;
        _lastPos = player.Camera.GlobalPosition;
    }

    public void _on_body_exited(PhysicsBody3D body)
    {
        GD.Print("Player exited");
        _player = null;
    }

    private int NonZeroSign(float f)
    {
        int s = Mathf.Sign(f);
        if (s == 0)
        {
            return 1;
        }

        return s;
    }
}
