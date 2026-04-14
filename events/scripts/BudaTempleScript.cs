using Godot;
using System.Collections.Generic;

public partial class BudaTempleScript : EventScript
{
    public override List<EventOptionData> get_options()
    {
        var option1 = new EventOptionData("Enter the temple", 0);
        var option2 = new EventOptionData("Leave it be", 1);
        return new List<EventOptionData> { option1, option2 };
    }

    public override void handle_response(object data)
    {
        int selectedOption = data is int intValue ? intValue : -1;
        if (selectedOption != 0)
        {
            return;
        }

        RunContext runContext = this.GetRunContext();
        if (runContext == null)
        {
            return;
        }

        string relicId = GD.Randf() < 0.5f ? "buda" : "cursed_buda";
        RelicData relicData = DataLoaderAccess.GetRelicById(relicId);
        Relic relic = relicData?.create_item();
        if (relic != null)
        {
            runContext.relics_manager.add_relic(relic);
        }
    }
}
