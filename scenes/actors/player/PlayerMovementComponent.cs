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

        Vector3 vel = Vector3.Zero;
        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            vel = direction * _speed;
            //vel += _player.Transform.Basis.X.Normalized() * direction * _speed;
            //vel += _player.Transform.Basis.Z.Normalized() * direction * _speed;
            //vel.X = direction.X * _speed;
            //vel.Z = direction.Z * _speed;
            //vel = _player.GlobalTransform.Basis * direction * _speed;
        }
        else
        {
            if (vel.X != 0.0f)
            {
                vel.X = Mathf.MoveToward(_player.Velocity.X, 0.0f, (float)delta * _acceleration);
            }

            if (vel.Y != 0.0f)
            {
                vel.Y = Mathf.MoveToward(_player.Velocity.Y, 0.0f, (float)delta * _acceleration);
            }

            if (vel.Z != 0.0f)
            {
                vel.Z = Mathf.MoveToward(_player.Velocity.Z, 0.0f, (float)delta * _acceleration);
            }
        }

        Vector3 oldVel = _player.Velocity;
        _player.Velocity =
            vel
            + (oldVel * _player.GlobalTransform.Basis.Y * _player.GlobalTransform.Basis.Y.Sign());

        if (_player.IsOnFloor())
        {
            if (Input.IsActionJustPressed("jump"))
            {
                _player.Velocity +=
                    (oldVel * _player.GlobalTransform.Basis.Y)
                    + _player.GlobalTransform.Basis.Y.Sign() * 4.5f;
            }
        }
        else
        {
            _player.Velocity =
                _player.Velocity
                - (_player.GlobalTransform.Basis.Y.Normalized() * 9.82f * (float)delta);
        }
        _player.MoveAndSlide();
    }
}
