using Godot;
using System.Collections.Generic;

public partial class PotionsEventScript : EventScript
{
    private const int CONSUMABLE_TYPE_POTION = 1;

    public override List<EventOptionData> get_options()
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
            string displayName = consumableData.display_name;
            options.Add(new EventOptionData(displayName, consumableData));
        }

        return options;
    }

    public override void handle_response(object data)
    {
        RunContext runContext = this.GetRunContext();
        if (runContext == null)
        {
            return;
        }

        ConsumableData consumableData = data as ConsumableData;
        Consumable consumable = consumableData?.create_consumable();
        if (consumable != null)
        {
            runContext.consumables_manager.add_consumable(consumable);
        }
    }
}
