using Godot;
using System.Collections.Generic;

public partial class LibraryEventScript : EventScript
{
    public override IReadOnlyList<EventOptionData> GetOptions()
    {
        RunContext runContext = GetRunContext();
        List<RelicData> allRelics = DataLoaderAccess.GetNotUsedRelicsTyped();
        var options = new List<EventOptionData>();

        foreach (RelicData relicData in allRelics)
        {
            bool isTome = relicData.IsTome;
            string relicId = relicData.Id;
            if (!isTome || runContext.RelicsManager.HasRelic(relicId))
            {
                continue;
            }

            string displayName = relicData.DisplayName;
            options.Add(new EventOptionData($"Acquire the {displayName}", EventOptionValue.FromRelicData(relicData)));
        }

        return options;
    }

    public override void HandleResponse(EventOptionValue data)
    {
        RunContext runContext = GetRunContext();
        RelicData relicData = data.RequireRelicData();
        Relic relic = relicData.CreateItem();
        if (relic != null)
        {
            runContext.RelicsManager.AddRelic(relic);
        }
    }
}
