using Godot;

public partial class FountainsOfWishesScript : EventScript
{
    public override Godot.Collections.Array<Variant> get_options()
    {
        RunContext runContext = this.GetRunContext();
        DataLoader dataLoader = this.GetDataLoader();
        if (runContext == null || dataLoader == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        int gold = runContext.economy.gold;
        bool epicAvailable = dataLoader.get_not_used_relics(2, false).Count > 0;
        bool rareAvailable = dataLoader.get_not_used_relics(1, false).Count > 0;
        bool commonAvailable = dataLoader.get_not_used_relics(0, false).Count > 0;

        var option1 = new EventOptionData("Offer 50 coins (Receive a Common Relic)", 0, gold < 50 || !commonAvailable);
        var option2 = new EventOptionData("Offer 80 coins (Receive a Rare Relic)", 1, gold < 80 || !rareAvailable);
        var option3 = new EventOptionData("Offer 120 coins (Receive a Epic Relic)", 2, gold < 120 || !epicAvailable);

        return new Godot.Collections.Array<Variant> { option1, option2, option3 };
    }

    public override void handle_response(Variant data)
    {
        int rarity = data.AsInt32();

        RunContext runContext = this.GetRunContext();
        DataLoader dataLoader = this.GetDataLoader();
        if (runContext == null || dataLoader == null)
        {
            return;
        }

        Godot.Collections.Array<Variant> relics = dataLoader.get_not_used_relics(rarity, false);
        if (relics.Count == 0)
        {
            return;
        }

        int randomIndex = (int)(GD.Randi() % (uint)relics.Count);
        GodotObject relicData = relics[randomIndex].AsGodotObject();
        Variant relic = relicData?.Call("create_item") ?? default;
        if (relic.VariantType == Variant.Type.Nil)
        {
            return;
        }

        runContext.relics_manager.add_relic(relic);

        int goldCost = rarity switch
        {
            0 => 50,
            1 => 80,
            2 => 120,
            _ => 0,
        };

        runContext.economy.spend_gold(goldCost);
    }
}
