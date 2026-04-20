using Godot;
using System.Collections.Generic;

public partial class FountainsOfWishesScript : EventScript
{
    public override List<EventOptionData> GetOptions()
    {
        RunContext runContext = GetRunContext();
        if (runContext == null)
        {
            return new List<EventOptionData>();
        }

        int gold = runContext.Economy.Gold;
        bool epicAvailable = DataLoaderAccess.GetNotUsedRelicsTyped(2, false).Count > 0;
        bool rareAvailable = DataLoaderAccess.GetNotUsedRelicsTyped(1, false).Count > 0;
        bool commonAvailable = DataLoaderAccess.GetNotUsedRelicsTyped(0, false).Count > 0;

        var option1 = new EventOptionData("Offer 50 coins (Receive a Common Relic)", 0, gold < 50 || !commonAvailable);
        var option2 = new EventOptionData("Offer 80 coins (Receive a Rare Relic)", 1, gold < 80 || !rareAvailable);
        var option3 = new EventOptionData("Offer 120 coins (Receive a Epic Relic)", 2, gold < 120 || !epicAvailable);

        return new List<EventOptionData> { option1, option2, option3 };
    }

    public override void HandleResponse(object data)
    {
        int rarity = data is int intValue ? intValue : 0;

        RunContext runContext = GetRunContext();
        if (runContext == null)
        {
            return;
        }

        List<RelicData> relics = DataLoaderAccess.GetNotUsedRelicsTyped(rarity, false);
        if (relics.Count == 0)
        {
            return;
        }

        int randomIndex = (int)(GD.Randi() % (uint)relics.Count);
        RelicData relicData = relics[randomIndex];
        Relic relic = relicData?.CreateItem();
        if (relic == null)
        {
            return;
        }

        runContext.RelicsManager.AddRelic(relic);

        int goldCost = rarity switch
        {
            0 => 50,
            1 => 80,
            2 => 120,
            _ => 0,
        };

        runContext.Economy.SpendGold(goldCost);
    }
}
