using Godot;
using System.Collections.Generic;

public partial class ChestEventScript : EventScript
{
    public override IReadOnlyList<EventOptionData> GetOptions()
    {
        var option1 = new EventOptionData("Open the chest", EventOptionValue.FromInt(0));
        var option2 = new EventOptionData("Leave it alone", EventOptionValue.FromInt(1));
        return new List<EventOptionData> { option1, option2 };
    }

    public override void HandleResponse(EventOptionValue data)
    {
        int selectedOption = data.RequireInt();
        if (selectedOption != 0)
        {
            return;
        }

        RunContext runContext = GetRunContext();

        List<RelicData> relics = DataLoaderAccess.GetNotUsedRelicsTyped(0, false);
        if (relics.Count == 0)
        {
            return;
        }

        int randomIndex = (int)(GD.Randi() % (uint)relics.Count);
        RelicData relicData = relics[randomIndex];
        Relic relic = relicData.CreateItem();
        if (relic != null)
        {
            runContext.RelicsManager.AddRelic(relic);
        }
    }
}
