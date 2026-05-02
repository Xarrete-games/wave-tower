using Godot;
using System.Collections.Generic;

public partial class BudaTempleScript : EventScript
{
    public override IReadOnlyList<EventOptionData> GetOptions()
    {
        var option1 = new EventOptionData("Enter the temple", 0);
        var option2 = new EventOptionData("Leave it be", 1);
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

        string relicId = GD.Randf() < 0.5f ? "buda" : "cursed_buda";
        RelicData relicData = DataLoaderAccess.GetRelicById(relicId);
        Relic relic = relicData?.CreateItem();
        if (relic != null)
        {
            runContext.RelicsManager.AddRelic(relic);
        }
    }
}
