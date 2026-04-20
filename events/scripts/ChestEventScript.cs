using Godot;
using System.Collections.Generic;

public partial class ChestEventScript : EventScript
{
    public override List<EventOptionData> GetOptions()
    {
        var option1 = new EventOptionData("Open the chest", 0);
        var option2 = new EventOptionData("Leave it alone", 1);
        return new List<EventOptionData> { option1, option2 };
    }

    public override void HandleResponse(object data)
    {
        int selectedOption = data is int intValue ? intValue : -1;
        if (selectedOption != 0)
        {
            return;
        }

        RunContext runContext = GetRunContext();
        if (runContext == null)
        {
            return;
        }

        List<RelicData> relics = DataLoaderAccess.GetNotUsedRelicsTyped(0, false);
        if (relics.Count == 0)
        {
            return;
        }

        int randomIndex = (int)(GD.Randi() % (uint)relics.Count);
        RelicData relicData = relics[randomIndex];
        Relic relic = relicData?.CreateItem();
        if (relic != null)
        {
            runContext.relics_manager.add_relic(relic);
        }
    }
}
