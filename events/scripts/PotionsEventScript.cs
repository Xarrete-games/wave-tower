using Godot;
using System.Collections.Generic;

public partial class PotionsEventScript : EventScript
{
    private const int CONSUMABLE_TYPE_POTION = 1;

    public override IReadOnlyList<EventOptionData> GetOptions()
    {
        List<ConsumableData> consumables = DataLoaderAccess.GetAllConsumablesByTypeTyped(CONSUMABLE_TYPE_POTION);
        if (consumables.Count == 0)
        {
            return new List<EventOptionData>();
        }

        var rng = new RandomNumberGenerator();
        for (int index = consumables.Count - 1; index > 0; index--)
        {
            int swapIndex = rng.RandiRange(0, index);
            ConsumableData tmp = consumables[index];
            consumables[index] = consumables[swapIndex];
            consumables[swapIndex] = tmp;
        }

        var options = new List<EventOptionData>();
        int count = Mathf.Min(3, consumables.Count);
        for (int index = 0; index < count; index++)
        {
            ConsumableData consumableData = consumables[index];
            string displayName = consumableData.DisplayName;
            options.Add(new EventOptionData(displayName, EventOptionValue.FromConsumableData(consumableData)));
        }

        return options;
    }

    public override void HandleResponse(EventOptionValue data)
    {
        RunContext runContext = GetRunContext();
        ConsumableData consumableData = data.RequireConsumableData();
        Consumable consumable = consumableData.CreateConsumable();
        if (consumable != null)
        {
            runContext.ConsumablesManager.AddConsumable(consumable);
        }
    }
}
