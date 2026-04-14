using Godot;
using System.Collections.Generic;

public partial class LibraryEventScript : EventScript
{
    public override List<EventOptionData> get_options()
    {
        RunContext runContext = this.GetRunContext();
        if (runContext == null)
        {
            return new List<EventOptionData>();
        }

        List<RelicData> allRelics = DataLoaderAccess.GetNotUsedRelicsTyped();
        var options = new List<EventOptionData>();

        foreach (RelicData relicData in allRelics)
        {
            bool isTome = relicData.is_tome;
            string relicId = relicData.id;
            if (!isTome || runContext.relics_manager.has_relic(relicId))
            {
                continue;
            }

            string displayName = relicData.display_name;
            options.Add(new EventOptionData($"Acquire the {displayName}", relicData));
        }

        return options;
    }

    public override void handle_response(object data)
    {
        RunContext runContext = this.GetRunContext();
        RelicData relicData = data as RelicData;
        if (runContext == null || relicData == null)
        {
            return;
        }

        Relic relic = relicData.create_item();
        if (relic != null)
        {
            runContext.relics_manager.add_relic(relic);
        }
    }
}
