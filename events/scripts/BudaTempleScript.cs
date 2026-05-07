using Godot;
using System.Collections.Generic;

public partial class BudaTempleScript : EventScript
{
    public override IReadOnlyList<EventOptionData> GetOptions()
    {
        var option1 = new EventOptionData("Enter the temple", EventOptionValue.FromInt(0));
        var option2 = new EventOptionData("Leave it be", EventOptionValue.FromInt(1));
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

        string relicId = GD.Randf() < 0.5f ? "buda" : "cursed_buda";
        RelicData relicData = DataLoaderAccess.GetRelicById(relicId);
        Relic relic = relicData.CreateItem();
        if (relic != null)
        {
            runContext.RelicsManager.AddRelic(relic);
        }
    }
}
