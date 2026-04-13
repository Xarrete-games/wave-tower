using Godot;
using System.Collections.Generic;

public partial class HighwayRobberyScript : EventScript
{
    public override List<EventOptionData> get_options()
    {
        RunContext runContext = this.GetRunContext();
        if (runContext == null)
        {
            return new List<EventOptionData>();
        }

        List<Relic> relics = runContext.relics_manager.get_all_relics();

        var options = new List<EventOptionData>();
        int count = Mathf.Min(3, relics.Count);
        for (int index = 0; index < count; index++)
        {
            int randomIndex = (int)(GD.Randi() % (uint)relics.Count);
            Relic relic = relics[randomIndex];
            relics.RemoveAt(randomIndex);
            string displayName = relic?.Data?.display_name ?? "relic";
            options.Add(new EventOptionData($"Give {displayName}.", Variant.From(relic?.Id ?? string.Empty)));
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

        string relicId = data.AsString();
        if (!string.IsNullOrEmpty(relicId))
        {
            runContext.relics_manager.remove_relic(relicId);
        }
    }
}
