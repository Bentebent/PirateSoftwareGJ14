using Godot;

public partial class PlayerOrthograpicCameraComponent : Node
{
    [Export]
    private Camera3D _camera = null;

    [Export]
    private float _sensitivity = 0.0f;

    [Export]
    private float _maxZoom = 0.0f;

    [Export]
    private float _minZoom = 0.0f;

    [Export]
    private Node3D _rootNode = null;

    private float _zoomTarget = 0.0f;
    private float _zoomDelta = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _zoomTarget = _camera.Size;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        HandleZoom();
        HandleRotation();
    }

    private void HandleRotation() {
        if (Input.IsActionJustPressed("move_left")) {
            _rootNode.RotateY(Mathf.Pi / 2.0f);
        } else if (Input.IsActionJustPressed("move_right")) {
            _rootNode.RotateY(-Mathf.Pi / 2.0f);
        }
    }

    private void HandleZoom()
    {
        if (Input.IsActionJustReleased("zoom_in"))
        {
            _zoomTarget -= 1.0f;
        }
        else if (Input.IsActionJustReleased("zoom_out"))
        {
            _zoomTarget += 1.0f;
        }

        _zoomTarget = Mathf.Clamp(_zoomTarget, _minZoom, _maxZoom);

        if (
            _camera.Size + Mathf.Epsilon < _zoomTarget
            || _camera.Size - Mathf.Epsilon > _zoomTarget
        )
        {
            _camera.Size = Mathf.Clamp(
                (float)Mathf.Lerp(_camera.Size, _zoomTarget, _zoomDelta),
                _minZoom,
                _maxZoom
            );
            _zoomDelta += 0.01f;
        }
        else
        {
            _zoomDelta = 0.0f;
            _camera.Size = _zoomTarget;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            InputEventMouseMotion mouseMotion = (InputEventMouseMotion)@event;
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
