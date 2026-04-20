using Godot;
using System.Collections.Generic;

public partial class LibraryEventScript : EventScript
{
    public override List<EventOptionData> GetOptions()
    {
        RunContext runContext = GetRunContext();
        if (runContext == null)
        {
            return new List<EventOptionData>();
        }

        List<RelicData> allRelics = DataLoaderAccess.GetNotUsedRelicsTyped();
        var options = new List<EventOptionData>();

        foreach (RelicData relicData in allRelics)
        {
            bool isTome = relicData.IsTome;
            string relicId = relicData.Id;
            if (!isTome || runContext.relics_manager.has_relic(relicId))
            {
                continue;
            }

            string displayName = relicData.DisplayName;
            options.Add(new EventOptionData($"Acquire the {displayName}", relicData));
        }

        return options;
    }

    public override void HandleResponse(object data)
    {
        RunContext runContext = GetRunContext();
        RelicData relicData = data as RelicData;
        if (runContext == null || relicData == null)
        {
            return;
        }

        Relic relic = relicData.CreateItem();
        if (relic != null)
        {
            runContext.relics_manager.add_relic(relic);
        }
    }
}
