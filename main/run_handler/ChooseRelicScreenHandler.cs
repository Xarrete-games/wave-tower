using Godot;
using System.Threading.Tasks;

public partial class ChooseRelicScreenHandler : Node
{
    private const int DefaultNumberOfRelics = 3;
    private const int RerollPrice = 20;

    private static readonly PackedScene RewardsScreenScene = GD.Load<PackedScene>("uid://bcxsfb0ox3gmq");

    private ChooseRelicScreen _rewardsScreen;

    public async Task ShowChooseRelicEventAsync(CanvasLayer eventLayer)
    {
        _rewardsScreen = RewardsScreenScene.Instantiate<ChooseRelicScreen>();

        int numberOfRelics = GetCurrentRewardsCount();
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        var relics = dataLoader.GetRandomAvailableRelics(numberOfRelics);

        eventLayer.AddChild(_rewardsScreen);
        _rewardsScreen.SetRelics(relics);
        _rewardsScreen.ItemSelected += OnItemSelected;
        _rewardsScreen.RerollPressed += OnRerollPressed;

        await ToSignal(_rewardsScreen, Node.SignalName.TreeExited);
        _rewardsScreen.ItemSelected -= OnItemSelected;
        _rewardsScreen.RerollPressed -= OnRerollPressed;
        _rewardsScreen = null;
    }

    private void OnItemSelected(RelicData selectedRelicData)
    {
        _rewardsScreen?.QueueFree();
        if (selectedRelicData == null)
        {
            return;
        }

        int healthPrice = selectedRelicData.HealthPrice;
        var runContext = GetNode<RunContext>("/root/RunContext");
        if (healthPrice > 0)
        {
            runContext.Status.Health -= healthPrice;
        }

        Relic item = selectedRelicData.CreateItem();
        runContext.RelicsManager.AddRelic(item);
    }

    private void OnRerollPressed()
    {
        var runContext = GetNode<RunContext>("/root/RunContext");
        runContext.Economy.Gold -= RerollPrice;

        int numberOfRelics = GetCurrentRewardsCount();
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        var relics = dataLoader.GetRandomAvailableRelics(numberOfRelics);
        _rewardsScreen?.SetRelics(relics);
    }

    private int GetCurrentRewardsCount()
    {
        var ctx = new RelicsRewardsContext(DefaultNumberOfRelics);
        Hooks.OnBeforeRelicReward(Hooks.GetListenersFromRuntime(), ctx);
        return ctx.NumberOfRelics;
    }
}