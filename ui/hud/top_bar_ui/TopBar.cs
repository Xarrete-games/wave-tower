using Godot;

public partial class TopBar : MarginContainer
{
    [Export]
    public XarretaButton SpeedButton;

    private GameState _gameState;

    public override void _Ready()
    {
        _gameState = GetNode<GameState>("/root/GameState");
        UpdateText(_gameState.speed);
        _gameState.speed_change += UpdateText;
    }

    public override void _ExitTree()
    {
        if (_gameState != null)
        {
            _gameState.speed_change -= UpdateText;
        }
    }

    private void OnXarretaTextButtonXarretaPressed()
    {
        ClickEvents.SpeedButtonPressed?.Invoke();
    }

    private void UpdateText(float value)
    {
        if (SpeedButton != null)
        {
            SpeedButton.Text = $"x{(int)value}";
        }
    }
}
