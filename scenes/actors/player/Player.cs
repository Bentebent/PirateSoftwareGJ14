using System;
using Godot;

public partial class Player : CharacterBody3D
{
    [Export]
    public Camera3D Camera = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("exit"))
        {
            GetTree().Quit();
        }
    }
}
