using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class Boot : Node
{
    private static readonly PackedScene PROCEDURAL_TEST = GD.Load<PackedScene>("uid://doun1k6w4e04m");

    public override void _Ready()
    {
        AsyncTaskHelper.FireAndForget(ReadyAsync(), "Boot.ReadyAsync");
    }

    private async Task ReadyAsync()
    {
        GameState gameState = GetNodeOrNull<GameState>("/root/GameState");
        RunContext runContext = RunContext.Instance;
        gameState?.ResetRun();
        runContext.ResetRun();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        GetTree().ChangeSceneToPacked(PROCEDURAL_TEST);
    }
}
