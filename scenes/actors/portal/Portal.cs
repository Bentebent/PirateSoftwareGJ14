using System;
using Godot;

public partial class Portal : MeshInstance3D
{
    [Export]
    private Portal _exitPortal = null;

    private SubViewport _viewport = null;
    private Camera3D _exitCamera = null;

    /*
    [Export]
    private Portal _sibling = null;

    [Export]
    private Camera3D _camera = null;

    [Export]
    private SubViewport _viewport = null;

    [Export]
    private MeshInstance3D _window = null;

    [Export]
    private Camera3D _playerCamera = null;

    private StandardMaterial3D _windowMaterial = null;
    */
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        /*
        _viewport.Size = GetViewport().GetWindow().Size;
        _camera.GlobalPosition =
            _window.GlobalPosition - (new Vector3(0, 0, 5) * _window.GlobalTransform.Basis.Z);

        _windowMaterial = new StandardMaterial3D();
        _window.SetSurfaceOverrideMaterial(0, _windowMaterial);

        _playerCamera = GetViewport().GetCamera3D();
        */

		//var materialOverride = GD.Load("res://materials/dev/portal.tres") as StandardMaterial3D;
		var materialOverride = new ShaderMaterial();
		materialOverride.Shader = GD.Load("res://assets/shaders/portal.gdshader") as Shader;

        SetSurfaceOverrideMaterial(0, materialOverride);

        CreateViewport(materialOverride);
    }

    private void CreateViewport(ShaderMaterial materialOverride)
    {
        _viewport = new SubViewport();

        _viewport.Name = "Viewport";
        _viewport.RenderTargetClearMode = SubViewport.ClearMode.Once;
        AddChild(_viewport);

        materialOverride.SetShaderParameter("albedo", _viewport.GetTexture());

        _exitCamera = new Camera3D
        {
            Name = "Camera",
            Environment = GetViewport().GetCamera3D().Environment
        };
        _viewport.AddChild(_exitCamera);

        var viewportSize = GetViewport().GetVisibleRect().Size;
        var aspectRatio = viewportSize.X / viewportSize.Y;
        _viewport.Size = new Vector2I((int)(512 * aspectRatio + 0.5), 512);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _exitCamera.GlobalTransform = ToExitTransform(GetViewport().GetCamera3D().GlobalTransform);


        var windowAABB = GetAabb();
        var cornerOne = _exitPortal.ToGlobal(
            new Vector3(windowAABB.Position.X, windowAABB.Position.Y, 0)
        );
        var cornerTwo = _exitPortal.ToGlobal(
            new Vector3(windowAABB.Position.X + windowAABB.Size.X, windowAABB.Position.Y, 0)
        );
        var cornerThree = _exitPortal.ToGlobal(
            new Vector3(
                windowAABB.Position.X + windowAABB.Size.X,
                windowAABB.Position.Y + windowAABB.Size.Y,
                0
            )
        );
        var cornerFour = _exitPortal.ToGlobal(
            new Vector3(windowAABB.Position.X, windowAABB.Position.Y + windowAABB.Size.Y, 0)
        );

        var cameraFwd = -_exitCamera.GlobalTransform.Basis.Z.Normalized();
        var a = (cornerOne - _exitCamera.GlobalPosition).Dot(cameraFwd);
        var b = (cornerTwo - _exitCamera.GlobalPosition).Dot(cameraFwd);
        var c = (cornerThree - _exitCamera.GlobalPosition).Dot(cameraFwd);
        var d = (cornerFour - _exitCamera.GlobalPosition).Dot(cameraFwd);

        _exitCamera.Near = Mathf.Max(0.01f, Mathf.Min(Mathf.Min(Mathf.Min(a, b), c), d) - 0.05f);
        _exitCamera.Far = GetViewport().GetCamera3D().Far;
        _exitCamera.Fov = GetViewport().GetCamera3D().Fov;
        _exitCamera.KeepAspect = GetViewport().GetCamera3D().KeepAspect;

        /*
        _windowMaterial.AlbedoTexture = _sibling.GetViewportTexture();
        var foo =
            _sibling.GlobalTransform
            * GlobalTransform.AffineInverse()
            * _playerCamera.GlobalTransform;
        _sibling.SetCameraTransform(foo);

        GD.Print(foo);

        return;

        var windowAABB = _window.GetAabb();
        var cornerOne = _sibling.ToGlobal(new Vector3(windowAABB.Position.X, windowAABB.Position.Y, 0));
        var cornerTwo = _sibling.ToGlobal(new Vector3(windowAABB.Position.X + windowAABB.Size.X, windowAABB.Position.Y, 0));
        var cornerThree = _sibling.ToGlobal(new Vector3(windowAABB.Position.X + windowAABB.Size.X, windowAABB.Position.Y + windowAABB.Size.Y, 0));
        var cornerFour = _sibling.ToGlobal(new Vector3(windowAABB.Position.X, windowAABB.Position.Y + windowAABB.Size.Y, 1));

        SetCameraClips(cornerOne, cornerTwo, cornerThree, cornerFour);
        */
    }

    private Transform3D ToExitTransform(Transform3D transform)
    {
        var local = GlobalTransform.AffineInverse() * transform;
        var unscaled = local.Scaled(GlobalTransform.Basis.Scale);
        var flipped = unscaled.Rotated(Vector3.Up, Mathf.Pi);
        var exitScaleAtVector = _exitPortal.GlobalTransform.Basis.Scale;
        var scaledAtExit = flipped.Scaled(Vector3.One / exitScaleAtVector * 1);

        return _exitPortal.GlobalTransform * scaledAtExit;
    }

    /*
    public ViewportTexture GetViewportTexture()
    {
        return _viewport.GetTexture();
    }

    public void SetCameraTransform(Transform3D targetTransform)
    {
        var bar = _camera.Transform;
        bar.Origin = targetTransform.Origin;
        //bar.Basis = new Basis(targetTransform.Basis.GetRotationQuaternion());

        _camera.Transform = bar;

        //_camera.GlobalTransform = Transform3D.Identity.Translated(targetTransform.Origin);
    }

    public void SetCameraClips(Vector3 one, Vector3 two, Vector3 three, Vector3 four) {
        var cameraFwd = -_camera.GlobalTransform.Basis.Z.Normalized();

        var a = (one - _camera.GlobalPosition).Dot(cameraFwd);
        var b = (two - _camera.GlobalPosition).Dot(cameraFwd);
        var c = (three - _camera.GlobalPosition).Dot(cameraFwd);
        var d = (four - _camera.GlobalPosition).Dot(cameraFwd);

        _camera.Near = Mathf.Max(0.1f, Mathf.Min(Mathf.Min(Mathf.Min(a, b), c), d)- 0.05f);
        _camera.Far = _playerCamera.Far;
        _camera.Fov = _playerCamera.Fov;
        _camera.KeepAspect = _playerCamera.KeepAspect;

    }
    */
}
