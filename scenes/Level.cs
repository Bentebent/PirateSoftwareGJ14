using System;
using System.Collections.Generic;
using Godot;

public partial class Level : Node3D
{
    [Export]
    private Vector3I _size = Vector3I.Zero;

	[Export]
	private Vector3 _cellSize = Vector3.Zero;

    private List<Node3D[,]> _grid = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _grid = new List<Node3D[,]>();

        for (int y = 0; y < _size.Y; y++)
        {
            _grid.Add(new Node3D[_size.X, _size.Z]);

            for (int x = -_size.X / 2; x < _size.X / 2; x++)
            {
                for (int z = -_size.Z / 2; z < _size.Z / 2; z++)
                {
                    var temp = new MeshInstance3D();
                    temp.Mesh = new BoxMesh();

                    AddChild(temp);

                    temp.GlobalPosition = new Vector3(x, _cellSize.Y * y, z);

                    _grid[y][x + _size.X / 2, z + _size.Z / 2] = temp;
                }
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
