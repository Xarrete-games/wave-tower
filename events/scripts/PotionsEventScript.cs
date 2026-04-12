using Godot;

public partial class PotionsEventScript : EventScript
{
    private const int CONSUMABLE_TYPE_POTION = 1;

    public override Godot.Collections.Array<Variant> get_options()
    {
        DataLoader dataLoader = this.GetDataLoader();
        if (dataLoader == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        Godot.Collections.Array<Variant> consumables = dataLoader.get_all_consumables_of_type(CONSUMABLE_TYPE_POTION);
        consumables.Shuffle();

        var options = new Godot.Collections.Array<Variant>();
        int count = Mathf.Min(3, consumables.Count);
        for (int index = 0; index < count; index++)
        {
            GodotObject consumableData = consumables[index].AsGodotObject();
            if (consumableData == null)
            {
                continue;
            }

            string displayName = consumableData.Get("display_name").AsString();
            Variant consumableItem = consumableData.Call("create_item");
            options.Add(new EventOptionData(displayName, consumableItem));
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

        runContext.consumables_manager.add_consumable(data);
    }
}
