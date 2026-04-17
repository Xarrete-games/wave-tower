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
        settings_section.Visible = false;
        menu_section.Visible = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("exit"))
        {
            if (settings_section.Visible)
            {
                settings_section.Visible = false;
                menu_section.Visible = true;
            }
            else
            {
                _ = Resume();
            }
        }
    }

    private async Task Resume()
    {
        GetTree().Paused = false;
        SceneTreeTimer timer = GetTree().CreateTimer(0.1f);
        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        resume_game?.Invoke();
    }

    public void pause()
    {
        GetTree().Paused = true;
    }

    private void OnResumeButtonXarretaPressed()
    {
        _ = Resume();
    }

    private void OnRestartButtonXarretaPressed()
    {
        GetTree().Paused = false;
        QueueFree();
        ClickEvents.ResetGameButtonPressed?.Invoke();
    }

    private void OnExitButtonXarretaPressed()
    {
        GetTree().Paused = false;
        QueueFree();

        GameState gameState = GetNode<GameState>("/root/GameState");
        gameState.state = GameState.ON_MAIN_MENU;
        GetTree().ChangeSceneToPacked(MainMenu);
    }

    private void OnBackButtonXarretaPressed()
    {
        settings_section.Visible = false;
        menu_section.Visible = true;
    }

    private void OnSettingsButtonXarretaPressed()
    {
        settings_section.Visible = true;
        menu_section.Visible = false;
    }
}
