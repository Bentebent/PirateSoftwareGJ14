using System;
using Godot;

public partial class PlayerMovementComponent : Node 
{
    [Export]
    private Player _player = null;

    [Export]
    private float _speed = 0.0f;

    [Export]
    private float _acceleration = 0.0f;

    private Vector3 _targetVelocity = Vector3.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public override void _PhysicsProcess(double delta)
    {
        var direction = Vector3.Zero;

        if (Input.IsActionPressed("move_fwd"))
        {
            direction -= _player.GlobalTransform.Basis.Z;
        }
        else if (Input.IsActionPressed("move_bwd"))
        {
            direction += _player.GlobalTransform.Basis.Z;
        }

        if (Input.IsActionPressed("move_left"))
        {
            direction -= _player.GlobalTransform.Basis.X;
        }
        else if (Input.IsActionPressed("move_right"))
        {
            direction += _player.GlobalTransform.Basis.X;
        }

        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
        }

        _targetVelocity.X = direction.X * _speed;
        _targetVelocity.Z = direction.Z * _speed;

        _player.Velocity = _player.Velocity.MoveToward(
            _targetVelocity,
            (float)(delta * _acceleration)
        );

        _player.MoveAndSlide();
    }
}
