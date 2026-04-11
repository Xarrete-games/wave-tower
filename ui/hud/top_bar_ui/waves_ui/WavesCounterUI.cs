using Godot;

public partial class WavesCounterUI : VBoxContainer
{
    private Label _levelLabel;
    private RunProgress _progress;

    public override void _Ready()
    {
        this._levelLabel = GetNode<Label>("HBoxContainer2/LevelLabel");

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._progress = runContext.progress;
        this._progress.current_wave_changed += this._on_wave_change;
    }

    public override void _ExitTree()
    {
        if (this._progress != null)
        {
            this._progress.current_wave_changed -= this._on_wave_change;
        }
    }

    private void _on_wave_change(int wave_num)
    {
        this._levelLabel.Text = wave_num.ToString();
    }
}
