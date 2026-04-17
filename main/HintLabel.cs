using Godot;

[GlobalClass]
public partial class HintLabel : Label
{
    private const string InitialHint = "[WASD] Move Camera";

    private bool _waitFirstHint = true;
    private GameState _gameState;

    public async override void _Ready()
    {
        Text = InitialHint;

        _gameState = GetNode<GameState>("/root/GameState");
        _gameState.state_change += OnStateChange;

        await ToSignal(GetTree().CreateTimer(10.0f, false), SceneTreeTimer.SignalName.Timeout);
        if (Text == InitialHint)
        {
            _waitFirstHint = false;
            Text = string.Empty;
        }
    }

    public override void _ExitTree()
    {
        if (_gameState != null)
        {
            _gameState.state_change -= OnStateChange;
        }
    }

    private void OnStateChange(int state)
    {
        if (state == GameState.IN_GAME && !_waitFirstHint)
        {
            Text = string.Empty;
        }
    }
}
