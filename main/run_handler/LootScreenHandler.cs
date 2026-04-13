using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class LootScreenHandler : Node
{
    private const int BaseGold = 50;
    private const int ExtraGoldPerWave = 5;
    private const int ChanceDropConsumable = 50;

    [Export]
    public PackedScene loot_screen_scene;

    public async Task ShowLootScreenAsync(CanvasLayer eventLayer)
    {
        if (this.loot_screen_scene == null)
        {
            GD.PushError("[LootScreenHandler] loot_screen_scene is null.");
            return;
        }

        LootScreen lootScreen = this.loot_screen_scene.Instantiate<LootScreen>();
        eventLayer.AddChild(lootScreen);

        var lootItems = this.GenerateLootItems();
        lootScreen.SetLoot(lootItems);

        await ToSignal(lootScreen, "tree_exited");
    }

    public List<LootItemData> GenerateLootItems()
    {
        var lootItems = new List<LootItemData>();

        LootItemData goldItem = new LootItemData();
        LootContext lootContext = new LootContext(this.GetBaseGold(), ChanceDropConsumable);
        Hooks.OnBeforeGetLoot(Hooks.GetListenersFromRuntime(), lootContext);
        goldItem.GoldAmount = lootContext.GetTotalGold();

        lootItems.Add(goldItem);
        if (GD.Randi() % 100 >= lootContext.ChanceDropConsumable)
        {
            return lootItems;
        }

        LootItemData consumableItem = new LootItemData();

        var consumables = DataLoaderAccess.GetAllConsumables();
        consumables.Shuffle();
        if (consumables.Count > 0)
        {
            consumableItem.Consumable = consumables[0].AsGodotObject();
            lootItems.Add(consumableItem);
        }

        return lootItems;
    }

    private int GetBaseGold()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        return BaseGold + (runContext.progress.current_wave * ExtraGoldPerWave);
    }
}