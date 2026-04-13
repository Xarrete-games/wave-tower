using Godot;

public partial class BudaTempleScript : EventScript
{
    public override Godot.Collections.Array<Variant> get_options()
    {
        var option1 = new EventOptionData("Enter the temple", 0);
        var option2 = new EventOptionData("Leave it be", 1);
        return new Godot.Collections.Array<Variant> { option1, option2 };
    }

    public override void handle_response(Variant data)
    {
        if (data.AsInt32() != 0)
        {
            return;
        }

        RunContext runContext = this.GetRunContext();
        DataLoader dataLoader = this.GetDataLoader();
        if (runContext == null || dataLoader == null)
        {
            return;
        }

        string relicId = GD.Randf() < 0.5f ? "buda" : "cursed_buda";
        RelicData relicData = dataLoader.get_relic_by_id(relicId).As<RelicData>();
        Relic relic = relicData?.create_item();
        if (relic != null)
        {
            runContext.relics_manager.add_relic(relic);
        }
    }
}
