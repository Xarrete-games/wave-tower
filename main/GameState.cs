using Godot;

public partial class GameState : Node
{
    [Signal]
    public delegate void state_changeEventHandler(int state);

    [Signal]
    public delegate void speed_changeEventHandler(float value);

    public const int ON_MAIN_MENU = 0;
    public const int IN_GAME = 1;

    private float _speed = 1.0f;
    private int _state = ON_MAIN_MENU;

    [Export]
    public float speed
    {
        get => this._speed;
        set
        {
            this._speed = value;
            Engine.TimeScale = value;
            this.EmitSignal(SignalName.speed_change, value);
        }
    }

    [Export]
    public int state
    {
        get => this._state;
        set
        {
            this._state = value;
            this.EmitSignal(SignalName.state_change, value);
        }
    }

    public override void _Ready()
    {
        Node clickEvents = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/ClickEvents");
        clickEvents?.Connect("speed_button_pressed", Callable.From(this._button_speed_pressed));
    }

    public bool is_on_main_menu()
    {
        return this.state == ON_MAIN_MENU;
    }

    public bool is_in_game()
    {
        return this.state == IN_GAME;
    }

    public void reset_run()
    {
        this.speed = 1.0f;
    }

    private void _button_speed_pressed()
    {
        if (Mathf.IsEqualApprox(this.speed, 1.0f))
        {
            this.speed = 2.0f;
        }
        else if (Mathf.IsEqualApprox(this.speed, 2.0f))
        {
            this.speed = 4.0f;
        }
        else
        {
            this.speed = 1.0f;
        }
    }
}
