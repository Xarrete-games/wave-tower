using Godot;
using System.Collections.Generic;

public partial class PotionsEventScript : EventScript
{
    private const int CONSUMABLE_TYPE_POTION = 1;

    public override List<EventOptionData> get_options()
    {
        DataLoader dataLoader = this.GetDataLoader();
        if (dataLoader == null)
        {
            return new List<EventOptionData>();
        }

        Godot.Collections.Array<Variant> consumables = dataLoader.get_all_consumables_of_type(CONSUMABLE_TYPE_POTION);
        consumables.Shuffle();

        var options = new List<EventOptionData>();
        int count = Mathf.Min(3, consumables.Count);
        for (int index = 0; index < count; index++)
        {
            ConsumableData consumableData = consumables[index].AsGodotObject() as ConsumableData;
            if (consumableData == null)
            {
                continue;
            }

            string displayName = consumableData.display_name;
            options.Add(new EventOptionData(displayName, Variant.From(consumableData)));
        }

        return options;
    }

    public override void handle_response(Variant data)
    {
        RunContext runContext = this.GetRunContext();
        if (runContext == null)
        {
            return;
        }

        ConsumableData consumableData = data.AsGodotObject() as ConsumableData;
        Consumable consumable = consumableData?.create_consumable();
        if (consumable != null)
        {
            runContext.consumables_manager.add_consumable(consumable);
        }
    }
}
