using Godot;

[GlobalClass]
public partial class HintLabel : Label
{
    private const string InitialHint = "[WASD] Move Camera";

    private bool _waitFirstHint = true;
    private GameState _gameState;

    public async override void _Ready()
    {
        this.Text = InitialHint;

        this._gameState = GetNode<GameState>("/root/GameState");
        this._gameState.state_change += this._on_state_change;

        await ToSignal(GetTree().CreateTimer(10.0f, false), SceneTreeTimer.SignalName.Timeout);
        if (this.Text == InitialHint)
        {
            this._waitFirstHint = false;
            this.Text = string.Empty;
        }
    }

    public override void _ExitTree()
    {
        if (this._gameState != null)
        {
            this._gameState.state_change -= this._on_state_change;
        }
    }

    private void _on_state_change(int state)
    {
        if (state == GameState.IN_GAME && !this._waitFirstHint)
        {
            this.Text = string.Empty;
        }
    }
}
