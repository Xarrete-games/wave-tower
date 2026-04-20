using Godot;

public partial class TopBar : MarginContainer
{
    [Export]
    public XarretaButton SpeedButton;

    private GameState _gameState;

    public override void _Ready()
    {
        _gameState = GetNode<GameState>("/root/GameState");
        UpdateText(_gameState.Speed);
        _gameState.SpeedChanged += UpdateText;
    }

    public override void _ExitTree()
    {
        if (_gameState != null)
        {
            _gameState.SpeedChanged -= UpdateText;
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
