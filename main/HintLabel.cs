using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class HintLabel : Label
{
    private const string InitialHint = "[WASD] Move Camera";

    private bool _waitFirstHint = true;
    private GameState _gameState;

    public override void _Ready()
    {
        _ = ReadyAsync();
    }

    private async Task ReadyAsync()
    {
        Text = InitialHint;

        _gameState = GetNode<GameState>("/root/GameState");
        _gameState.StateChanged += OnStateChange;

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
            _gameState.StateChanged -= OnStateChange;
        }
    }

    private void OnStateChange(int state)
    {
        if (state == GameState.InGame && !_waitFirstHint)
        {
            Text = string.Empty;
        }
    }
}
