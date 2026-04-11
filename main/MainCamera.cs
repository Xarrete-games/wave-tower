using Godot;

[GlobalClass]
public partial class MainCamera : Camera2D
{
    private static readonly float[] ZoomSteps = { 0.5f, 0.75f, 1.0f };

    [Export]
    public float move_speed = 500.0f;

    [Export]
    public float zoom_tween_duration = 0.25f;

    [Export]
    public Node2D level;

    private int _zoomIndex;
    private Tween _zoomTween;
    private bool _isMiddleMousePanning;

    public override void _Ready()
    {
        this._zoomIndex = 0;
        float zoomValue = ZoomSteps[this._zoomIndex];
        this.Zoom = new Vector2(zoomValue, zoomValue);
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

        this.GlobalPosition += inputVector * this.move_speed * (float)delta;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Middle)
        {
            this._isMiddleMousePanning = mouseButton.Pressed;
        }
        else if (@event is InputEventMouseMotion mouseMotion && this._isMiddleMousePanning)
        {
            this.GlobalPosition -= new Vector2(mouseMotion.Relative.X / this.Zoom.X, mouseMotion.Relative.Y / this.Zoom.Y);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mouseButton || !mouseButton.Pressed)
        {
            return;
        }

        if (this._zoomTween != null && this._zoomTween.IsRunning())
        {
            return;
        }

        if (mouseButton.ButtonIndex == MouseButton.WheelUp)
        {
            this._change_zoom_step(1);
        }
        else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
        {
            this._change_zoom_step(-1);
        }
    }

    private void _change_zoom_step(int direction)
    {
        int newIndex = Mathf.Clamp(this._zoomIndex + direction, 0, ZoomSteps.Length - 1);
        if (newIndex == this._zoomIndex)
        {
            return;
        }

        this._zoomIndex = newIndex;
        float target = ZoomSteps[this._zoomIndex];

        if (this._zoomTween != null && this._zoomTween.IsRunning())
        {
            this._zoomTween.Kill();
        }

        this._zoomTween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        this._zoomTween.TweenProperty(this, "zoom", new Vector2(target, target), this.zoom_tween_duration);
    }
}
