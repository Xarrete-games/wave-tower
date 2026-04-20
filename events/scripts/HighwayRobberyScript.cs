using Godot;
using System.Collections.Generic;

public partial class HighwayRobberyScript : EventScript
{
    public override List<EventOptionData> GetOptions()
    {
        RunContext runContext = GetRunContext();
        if (runContext == null)
        {
            return new List<EventOptionData>();
        }

        List<Relic> relics = runContext.relics_manager.GetAllRelics();

        var options = new List<EventOptionData>();
        int count = Mathf.Min(3, relics.Count);
        for (int index = 0; index < count; index++)
        {
            int randomIndex = (int)(GD.Randi() % (uint)relics.Count);
            Relic relic = relics[randomIndex];
            relics.RemoveAt(randomIndex);
            string displayName = relic?.Data?.DisplayName ?? "relic";
            options.Add(new EventOptionData($"Give {displayName}.", relic?.Id ?? string.Empty));
        }

        return options;
    }

    public override void HandleResponse(object data)
    {
        RunContext runContext = GetRunContext();
        if (runContext == null)
        {
            return;
        }

        string relicId = data as string;
        if (!string.IsNullOrEmpty(relicId))
        {
            runContext.relics_manager.RemoveRelic(relicId);
        }
    }
}
