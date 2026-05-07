using Godot;
using System;

public partial class GameState : Node
{
    public event Action<int> StateChanged;
    public event Action<float> SpeedChanged;

    public const int OnMainMenu = 0;
    public const int InGame = 1;

    private float _speed = 1.0f;
    private int _state = OnMainMenu;

    [Export]
    public float Speed
    {
        get => _speed;
        set
        {
            _speed = value;
            Engine.TimeScale = value;
            SpeedChanged?.Invoke(value);
        }
    }

    [Export]
    public int State
    {
        get => _state;
        set
        {
            _state = value;
            StateChanged?.Invoke(value);
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

    public bool IsOnMainMenu()
    {
        return State == OnMainMenu;
    }

    public bool IsInGame()
    {
        return State == InGame;
    }

    public void ResetRun()
    {
        Speed = 1.0f;
    }

    private void ButtonSpeedPressed()
    {
        if (Mathf.IsEqualApprox(Speed, 1.0f))
        {
            Speed = 2.0f;
        }
        else if (Mathf.IsEqualApprox(Speed, 2.0f))
        {
            Speed = 4.0f;
        }
        else
        {
            Speed = 1.0f;
        }
    }
}

