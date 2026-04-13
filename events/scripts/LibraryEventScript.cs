using Godot;
using System.Collections.Generic;

public partial class LibraryEventScript : EventScript
{
    public override List<EventOptionData> get_options()
    {
        RunContext runContext = this.GetRunContext();
        DataLoader dataLoader = this.GetDataLoader();
        if (runContext == null || dataLoader == null)
        {
            return new List<EventOptionData>();
        }

        Godot.Collections.Array<Variant> allRelics = dataLoader.get_not_used_relics();
        var options = new List<EventOptionData>();

        foreach (Variant relicVariant in allRelics)
        {
            GodotObject relicData = relicVariant.AsGodotObject();
            if (relicData == null)
            {
                continue;
            }

            bool isTome = (bool)relicData.Get("is_tome");
            string relicId = relicData.Get("id").AsString();
            if (!isTome || runContext.relics_manager.has_relic(relicId))
            {
                continue;
            }

            string displayName = relicData.Get("display_name").AsString();
            options.Add(new EventOptionData($"Acquire the {displayName}", relicVariant));
        }

        return options;
    }

    public override void handle_response(Variant data)
    {
        RunContext runContext = this.GetRunContext();
        GodotObject relicData = data.AsGodotObject();
        if (runContext == null || relicData == null)
        {
            return;
        }

        RelicData typedRelicData = relicData as RelicData;
        Relic relic = typedRelicData?.create_item();
        if (relic != null)
        {
            runContext.relics_manager.add_relic(relic);
        }
    }
}
