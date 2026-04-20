using Godot;

public partial class WavesCounterUI : VBoxContainer
{
    private Label _levelLabel;
    private RunProgress _progress;

    public override void _Ready()
    {
        _levelLabel = GetNode<Label>("HBoxContainer2/LevelLabel");

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _progress = runContext.Progress;
        _progress.CurrentWaveChanged += OnWaveChange;
    }

    public override void _ExitTree()
    {
        if (_progress != null)
        {
            _progress.CurrentWaveChanged -= OnWaveChange;
        }
    }

    private void OnWaveChange(int wave_num)
    {
        _levelLabel.Text = wave_num.ToString();
    }
}
