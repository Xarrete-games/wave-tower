using Godot;

public partial class TopBar : MarginContainer
{
    [Export]
    public XarretaButton speed_button;

    private GameState _gameState;

    public override void _Ready()
    {
        this._gameState = GetNode<GameState>("/root/GameState");
        this.UpdateText(this._gameState.speed);
        this._gameState.speed_change += this.UpdateText;
    }

    public override void _ExitTree()
    {
        if (this._gameState != null)
        {
            this._gameState.speed_change -= this.UpdateText;
        }
    }

    private void _on_xarreta_text_button_xarreta_pressed()
    {
        ClickEventsBus.EmitSpeedButtonPressed();
    }

    private void UpdateText(float value)
    {
        if (this.speed_button != null)
        {
            this.speed_button.Text = $"x{(int)value}";
        }
    }
}