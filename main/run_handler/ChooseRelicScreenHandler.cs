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
        var relics = dataLoader.get_random_available_relics(numberOfRelics);

        eventLayer.AddChild(_rewardsScreen);
        _rewardsScreen.set_relics(relics);
        _rewardsScreen.item_selected += OnItemSelected;
        _rewardsScreen.reroll_pressed += OnRerollPressed;

        await ToSignal(_rewardsScreen, Node.SignalName.TreeExited);
        _rewardsScreen.item_selected -= OnItemSelected;
        _rewardsScreen.reroll_pressed -= OnRerollPressed;
        _rewardsScreen = null;
    }

    private void OnItemSelected(Variant relicData)
    {
        _rewardsScreen?.QueueFree();

        RelicData selectedRelicData = relicData.As<RelicData>();
        if (selectedRelicData == null)
        {
            return;
        }

        int healthPrice = selectedRelicData.HealthPrice;
        var runContext = GetNode<RunContext>("/root/RunContext");
        if (healthPrice > 0)
        {
            runContext.status.health -= healthPrice;
        }

        Relic item = selectedRelicData.CreateItem();
        runContext.relics_manager.add_relic(item);
    }

    private void OnRerollPressed()
    {
        var runContext = GetNode<RunContext>("/root/RunContext");
        runContext.economy.gold -= RerollPrice;

        int numberOfRelics = GetCurrentRewardsCount();
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        var relics = dataLoader.get_random_available_relics(numberOfRelics);
        _rewardsScreen?.set_relics(relics);
    }

    private int GetCurrentRewardsCount()
    {
        var ctx = new RelicsRewardsContext(DefaultNumberOfRelics);
        Hooks.OnBeforeRelicReward(Hooks.GetListenersFromRuntime(), ctx);
        return ctx.NumberOfRelics;
    }
}