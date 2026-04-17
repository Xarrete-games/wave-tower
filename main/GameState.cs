using Godot;
using System;

public partial class GameState : Node
{
    public event Action<int> state_change;
    public event Action<float> speed_change;

    public const int ON_MAIN_MENU = 0;
    public const int IN_GAME = 1;

    private float _speed = 1.0f;
    private int _state = ON_MAIN_MENU;

    [Export]
    public float speed
    {
        get => _speed;
        set
        {
            _speed = value;
            Engine.TimeScale = value;
            speed_change?.Invoke(value);
        }
    }

    [Export]
    public int state
    {
        get => _state;
        set
        {
            _state = value;
            state_change?.Invoke(value);
        }
    }

    public override void _Ready()
    {
        ClickEvents.SpeedButtonPressed += ButtonSpeedPressed;
    }

    public override void _ExitTree()
    {
        ClickEvents.SpeedButtonPressed -= ButtonSpeedPressed;
    }

    public bool is_on_main_menu()
    {
        return state == ON_MAIN_MENU;
    }

    public bool is_in_game()
    {
        return state == IN_GAME;
    }

    public void reset_run()
    {
        speed = 1.0f;
    }

    private void ButtonSpeedPressed()
    {
        if (Mathf.IsEqualApprox(speed, 1.0f))
        {
            speed = 2.0f;
        }
        else if (Mathf.IsEqualApprox(speed, 2.0f))
        {
            speed = 4.0f;
        }
        else
        {
            speed = 1.0f;
        }
    }
}

