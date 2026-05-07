using Godot;
using System.Collections.Generic;

public partial class HighwayRobberyScript : EventScript
{
    public override IReadOnlyList<EventOptionData> GetOptions()
    {
        RunContext runContext = GetRunContext();
        List<Relic> relics = runContext.RelicsManager.GetAllRelics();

        var options = new List<EventOptionData>();
        int count = Mathf.Min(3, relics.Count);
        for (int index = 0; index < count; index++)
        {
            int randomIndex = (int)(GD.Randi() % (uint)relics.Count);
            Relic relic = relics[randomIndex];
            relics.RemoveAt(randomIndex);
            string displayName = relic?.Data?.DisplayName ?? "relic";
            options.Add(new EventOptionData($"Give {displayName}.", EventOptionValue.FromString(relic?.Id ?? string.Empty)));
        }

        return options;
    }

    public override void HandleResponse(EventOptionValue data)
    {
        RunContext runContext = GetRunContext();
        string relicId = data.RequireString();
        if (!string.IsNullOrEmpty(relicId))
        {
            runContext.RelicsManager.RemoveRelic(relicId);
        }
    }
}
