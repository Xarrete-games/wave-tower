using Godot;
using System.Collections.Generic;

public partial class BloodPactScript : EventScript
{
    public override List<EventOptionData> get_options()
    {
        var option1 = new EventOptionData("Sacrifice 15 of your health to gain a powerful relic.", true);
        var option2 = new EventOptionData("Walk away unharmed.", false);
        return new List<EventOptionData> { option1, option2 };
    }

    public override void handle_response(object data)
    {
        bool accepted = data is bool boolValue && boolValue;
        if (!accepted)
        {
            return;
        }

        RunContext runContext = GetRunContext();
        if (runContext == null)
        {
            return;
        }

        runContext.status.apply_damage(15);

        List<RelicData> allRelics = DataLoaderAccess.GetNotUsedRelicsTyped();
        if (allRelics.Count == 0)
        {
            return;
        }

        int randomIndex = (int)(GD.Randi() % (uint)allRelics.Count);
        RelicData relicData = allRelics[randomIndex];
        Relic relic = relicData?.create_item();
        if (relic != null)
        {
            runContext.relics_manager.add_relic(relic);
        }
    }
}
