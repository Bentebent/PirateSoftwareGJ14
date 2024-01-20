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
            vel.X = direction.X * _speed;
            vel.Z = direction.Z * _speed;
        }
        else
        {
            vel.X = Mathf.MoveToward(_player.Velocity.X, 0.0f, (float)delta * _acceleration);
            vel.Z = Mathf.MoveToward(_player.Velocity.Z, 0.0f, (float)delta * _acceleration);
        }

        //       float yVel = _player.Velocity.Y;
        //       if (_player.IsOnFloor())
        //       {
        //           if (Input.IsActionJustPressed("jump"))
        //           {
        //               yVel = 10.5f;
        //           }
        //       }
        //       else
        //       {
        //           yVel -= 9.82f;
        //       }

        //       _targetVelocity.X = direction.X * _speed;
        //       _targetVelocity.Z = direction.Z * _speed;

        //       _player.Velocity = _player.Velocity.MoveToward(
        //           _targetVelocity,
        //           (float)(delta * _acceleration)
        //       );

        //        _player.Velocity = new Vector3(_player.Velocity.X, yVel, _player.Velocity.Z);



        if (_player.IsOnFloor())
        {
            if (Input.IsActionJustPressed("jump"))
            {
                vel.Y = 4.5f;
            }
        }
        else
        {
            int up = Mathf.Sign(Vector3.Up.Dot(_player.GlobalTransform.Basis.Y));
            vel.Y = _player.Velocity.Y - up * 9.82f * (float)delta;
        }

        _player.Velocity = vel;
        _player.MoveAndSlide();
    }
}
