using Godot;
using System;
using System.Threading.Tasks;

public partial class PauseMenu : Control
{
    public event Action ResumeGame;

    private static readonly PackedScene MainMenu = GD.Load<PackedScene>("uid://4i6kl0xurgeg");

    [Export]
    public Control SettingsSection;

    [Export]
    public Control MenuSection;

    public override void _Ready()
    {
        SettingsSection.Visible = false;
        MenuSection.Visible = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("exit"))
        {
            if (SettingsSection.Visible)
            {
                SettingsSection.Visible = false;
                MenuSection.Visible = true;
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
        ResumeGame?.Invoke();
    }

    public void Pause()
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
        SettingsSection.Visible = false;
        MenuSection.Visible = true;
    }

    private void OnSettingsButtonXarretaPressed()
    {
        SettingsSection.Visible = true;
        MenuSection.Visible = false;
    }
}
