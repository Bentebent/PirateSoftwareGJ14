using System;
using System.Reflection.Metadata;
using Godot;

public partial class PlayerCameraComponent : Node
{
    [Export]
    private Player _player = null;

    [Export]
    private Camera3D _camera = null;

    [Export]
    private float _sensitivity = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            InputEventMouseMotion mouseMotion = (InputEventMouseMotion)@event;
            RotateMe(mouseMotion.Relative);
            _player.RotateY(-mouseMotion.Relative.X * _sensitivity);
        }
    }

    private void RotateMe(in Vector2 mouseMovement)
    {
        _camera.RotateX(-mouseMovement.Y * _sensitivity);
        _camera.Rotation = new Vector3(
            Mathf.Clamp(_camera.Rotation.X, -1.4f, 1.2f),
            _camera.Rotation.Y,
            _camera.Rotation.Z
        );
    }
}
