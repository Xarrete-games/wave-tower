using Godot;

public partial class TopBar : MarginContainer
{
    [Export]
    public Node speed_button;

    public override void _Ready()
    {
        GameState gameState = GetNode<GameState>("/root/GameState");
        this.UpdateText(gameState.speed);
        gameState.Connect("speed_change", Callable.From<float>(this.UpdateText));
    }

    private void _on_xarreta_text_button_xarreta_pressed()
    {
        ClickEventsBus.EmitSpeedButtonPressed();
    }

    private void UpdateText(float value)
    {
        this.speed_button?.Set("text", $"x{(int)value}");
    }
}