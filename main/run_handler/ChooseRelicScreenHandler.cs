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
        this._rewardsScreen = RewardsScreenScene.Instantiate<ChooseRelicScreen>();

        int numberOfRelics = this.GetCurrentRewardsCount();
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        var relics = dataLoader.get_random_available_relics(numberOfRelics);

        eventLayer.AddChild(this._rewardsScreen);
        this._rewardsScreen.set_relics(relics);
        this._rewardsScreen.item_selected += this.OnItemSelected;
        this._rewardsScreen.reroll_pressed += this.OnRerollPressed;

        await ToSignal(this._rewardsScreen, Node.SignalName.TreeExited);
        this._rewardsScreen.item_selected -= this.OnItemSelected;
        this._rewardsScreen.reroll_pressed -= this.OnRerollPressed;
        this._rewardsScreen = null;
    }

    private void OnItemSelected(Variant relicData)
    {
        this._rewardsScreen?.QueueFree();

        RelicData selectedRelicData = relicData.As<RelicData>();
        if (selectedRelicData == null)
        {
            return;
        }

        int healthPrice = selectedRelicData.health_price;
        var runContext = GetNode<RunContext>("/root/RunContext");
        if (healthPrice > 0)
        {
            runContext.status.health -= healthPrice;
        }

        Relic item = selectedRelicData.create_item();
        runContext.relics_manager.add_relic(item);
    }

    private void OnRerollPressed()
    {
        var runContext = GetNode<RunContext>("/root/RunContext");
        runContext.economy.gold -= RerollPrice;

        int numberOfRelics = this.GetCurrentRewardsCount();
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        var relics = dataLoader.get_random_available_relics(numberOfRelics);
        this._rewardsScreen?.set_relics(relics);
    }

    private int GetCurrentRewardsCount()
    {
        var ctx = new RelicsRewardsContext(DefaultNumberOfRelics);
        Hooks.OnBeforeRelicReward(Hooks.GetListenersFromRuntime(), ctx);
        return ctx.NumberOfRelics;
    }
}