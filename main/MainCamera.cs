using Godot;

[GlobalClass]
public partial class MainCamera : Camera2D
{
    private static readonly float[] ZoomSteps = { 0.5f, 0.75f, 1.0f };

    [Export]
    public float MoveSpeed = 500.0f;

    [Export]
    public float ZoomTweenDuration = 0.25f;

    [Export]
    public Node2D level;

    private int _zoomIndex;
    private Tween _zoomTween;
    private bool _isMiddleMousePanning;

    public override void _Ready()
    {
        _zoomIndex = 0;
        float zoomValue = ZoomSteps[_zoomIndex];
        Zoom = new Vector2(zoomValue, zoomValue);
    }

    public override void _Process(double delta)
    {
        Vector2 inputVector = Vector2.Zero;
        inputVector.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        inputVector.Y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

        if (inputVector != Vector2.Zero)
        {
            inputVector = inputVector.Normalized();
        }

        GlobalPosition += inputVector * MoveSpeed * (float)delta;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Middle)
        {
            _isMiddleMousePanning = mouseButton.Pressed;
        }
        else if (@event is InputEventMouseMotion mouseMotion && _isMiddleMousePanning)
        {
            GlobalPosition -= new Vector2(mouseMotion.Relative.X / Zoom.X, mouseMotion.Relative.Y / Zoom.Y);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mouseButton || !mouseButton.Pressed)
        {
            return;
        }

        if (_zoomTween != null && _zoomTween.IsRunning())
        {
            return;
        }

        if (mouseButton.ButtonIndex == MouseButton.WheelUp)
        {
            ChangeZoomStep(1);
        }
        else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
        {
            ChangeZoomStep(-1);
        }
    }

    private void ChangeZoomStep(int direction)
    {
        int newIndex = Mathf.Clamp(_zoomIndex + direction, 0, ZoomSteps.Length - 1);
        if (newIndex == _zoomIndex)
        {
            return;
        }

        _zoomIndex = newIndex;
        float target = ZoomSteps[_zoomIndex];

        if (_zoomTween != null && _zoomTween.IsRunning())
        {
            _zoomTween.Kill();
        }

        _zoomTween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        _zoomTween.TweenProperty(this, "zoom", new Vector2(target, target), ZoomTweenDuration);
    }
}
