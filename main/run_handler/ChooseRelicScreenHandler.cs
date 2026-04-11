using Godot;
using System.Threading.Tasks;

public partial class ChooseRelicScreenHandler : Node
{
    private const int DefaultNumberOfRelics = 3;
    private const int RerollPrice = 20;

    private static readonly PackedScene RewardsScreenScene = GD.Load<PackedScene>("uid://bcxsfb0ox3gmq");

    private Node _rewardsScreen;

    public async Task ShowChooseRelicEventAsync(CanvasLayer eventLayer)
    {
        this._rewardsScreen = RewardsScreenScene.Instantiate();

        int numberOfRelics = this.GetCurrentRewardsCount();
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        var relics = dataLoader.get_random_available_relics(numberOfRelics);

        eventLayer.AddChild(this._rewardsScreen);
        this._rewardsScreen.Call("set_relics", relics);
        this._rewardsScreen.Connect("item_selected", Callable.From<Variant>(this.OnItemSelected));
        this._rewardsScreen.Connect("reroll_pressed", Callable.From(this.OnRerollPressed));

        await ToSignal(this._rewardsScreen, "tree_exited");
        this._rewardsScreen = null;
    }

    private void OnItemSelected(Variant relicData)
    {
        this._rewardsScreen?.QueueFree();

        GodotObject relicDataObj = relicData.AsGodotObject();
        if (relicDataObj == null)
        {
            return;
        }

        int healthPrice = (int)relicDataObj.Get("health_price");
        if (healthPrice > 0)
        {
            var runContext = GetNode<RunContext>("/root/RunContext");
            int health = (int)runContext.status.Get("health");
            runContext.status.Set("health", health - healthPrice);
        }

        Variant item = relicDataObj.Call("create_item");
        GetNode<RunContext>("/root/RunContext").relics_manager.Call("add_relic", item);
    }

    private void OnRerollPressed()
    {
        var runContext = GetNode<RunContext>("/root/RunContext");
        int gold = (int)runContext.economy.Get("gold");
        runContext.economy.Set("gold", gold - RerollPrice);

        int numberOfRelics = this.GetCurrentRewardsCount();
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        var relics = dataLoader.get_random_available_relics(numberOfRelics);
        this._rewardsScreen?.Call("set_relics", relics);
    }

    private int GetCurrentRewardsCount()
    {
        var ctx = new RelicsRewardsContext(DefaultNumberOfRelics);
        Hooks.OnBeforeRelicReward(Hooks.GetListenersFromRuntime(), ctx);
        return ctx.NumberOfRelics;
    }
}