using Godot;
using System;
using System.Threading.Tasks;

public partial class PauseMenu : Control
{
    public event Action resume_game;

    private static readonly PackedScene MainMenu = GD.Load<PackedScene>("uid://4i6kl0xurgeg");

    [Export]
    public Control settings_section;

    [Export]
    public Control menu_section;

    public override void _Ready()
    {
        this.settings_section.Visible = false;
        this.menu_section.Visible = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("exit"))
        {
            if (this.settings_section.Visible)
            {
                this.settings_section.Visible = false;
                this.menu_section.Visible = true;
            }
            else
            {
                _ = this.Resume();
            }
        }
    }

    private async Task Resume()
    {
        GetTree().Paused = false;
        SceneTreeTimer timer = GetTree().CreateTimer(0.1f);
        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        this.resume_game?.Invoke();
    }

    public void pause()
    {
        GetTree().Paused = true;
    }

    private void _on_resume_button_xarreta_pressed()
    {
        _ = this.Resume();
    }

    private void _on_restart_button_xarreta_pressed()
    {
        GetTree().Paused = false;
        QueueFree();
        ClickEvents.ResetGameButtonPressed?.Invoke();
    }

    private void _on_exit_button_xarreta_pressed()
    {
        GetTree().Paused = false;
        QueueFree();

        GameState gameState = GetNode<GameState>("/root/GameState");
        gameState.state = GameState.ON_MAIN_MENU;
        GetTree().ChangeSceneToPacked(MainMenu);
    }

    private void _on_back_button_xarreta_pressed()
    {
        this.settings_section.Visible = false;
        this.menu_section.Visible = true;
    }

    private void _on_settings_button_xarreta_pressed()
    {
        this.settings_section.Visible = true;
        this.menu_section.Visible = false;
    }
}
