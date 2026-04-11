using Godot;
using System.Threading.Tasks;

public partial class LootScreenHandler : Node
{
    private const int BaseGold = 50;
    private const int ExtraGoldPerWave = 5;
    private const int ChanceDropConsumable = 50;

    [Export]
    public PackedScene loot_screen_scene;

    private static readonly Script LootItemDataScript = GD.Load<Script>("res://main/run_handler/loot_item_data.gd");
    private static readonly Script LootContextScript = GD.Load<Script>("res://core/context/loot_context.gd");
    private static readonly Script HooksScript = GD.Load<Script>("res://core/hooks.gd");

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

    public Godot.Collections.Array<Variant> GenerateLootItems()
    {
        var lootItems = new Godot.Collections.Array<Variant>();

        if (LootItemDataScript == null || LootContextScript == null || HooksScript == null)
        {
            GD.PushError("[LootScreenHandler] Missing loot scripts (loot_item_data, loot_context, or hooks).");
            return lootItems;
        }

        GodotObject goldItem = LootItemDataScript.Call("new").AsGodotObject();
        GodotObject lootContext = LootContextScript.Call("new", this.GetBaseGold(), ChanceDropConsumable).AsGodotObject();
        if (goldItem == null || lootContext == null)
        {
            GD.PushError("[LootScreenHandler] Could not instantiate loot data/context scripts.");
            return lootItems;
        }

        HooksScript.Call("on_before_get_loot", lootContext);
        goldItem.Set("gold_amount", (int)lootContext.Call("get_total_gold"));

        lootItems.Add(goldItem);
        if (GD.Randi() % 100 >= (int)lootContext.Get("chance_drop_consumable"))
        {
            return lootItems;
        }

        GodotObject consumableItem = LootItemDataScript.Call("new").AsGodotObject();
        if (consumableItem == null)
        {
            return lootItems;
        }

        Node dataLoader = GetNodeOrNull<Node>("/root/DataLoader");
        if (dataLoader == null)
        {
            GD.PushError("[LootScreenHandler] DataLoader singleton not found.");
            return lootItems;
        }

        var consumables = dataLoader.Call("get_all_consumables").AsGodotArray<Variant>();
        consumables.Shuffle();
        if (consumables.Count > 0)
        {
            consumableItem.Set("consumable", consumables[0]);
            lootItems.Add(consumableItem);
        }

        return lootItems;
    }

    private int GetBaseGold()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        return BaseGold + ((int)runContext.progress.Get("current_wave") * ExtraGoldPerWave);
    }
}