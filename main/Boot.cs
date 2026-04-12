using Godot;

[GlobalClass]
public partial class Boot : Node
{
    private static readonly PackedScene PROCEDURAL_TEST = GD.Load<PackedScene>("uid://doun1k6w4e04m");

    public override async void _Ready()
    {
        GameState gameState = GetNodeOrNull<GameState>("/root/GameState");
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        gameState?.reset_run();
        runContext?.reset_run();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        GetTree().ChangeSceneToPacked(PROCEDURAL_TEST);
    }
}
