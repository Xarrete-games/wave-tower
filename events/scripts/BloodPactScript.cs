using Godot;

public partial class BloodPactScript : EventScript
{
    public override Godot.Collections.Array<Variant> get_options()
    {
        var option1 = new EventOptionData("Sacrifice 15 of your health to gain a powerful relic.", true);
        var option2 = new EventOptionData("Walk away unharmed.", false);
        return new Godot.Collections.Array<Variant> { option1, option2 };
    }

    public override void handle_response(Variant data)
    {
        if (!data.AsBool())
        {
            return;
        }

        RunContext runContext = this.GetRunContext();
        DataLoader dataLoader = this.GetDataLoader();
        if (runContext == null || dataLoader == null)
        {
            return;
        }

        runContext.status.apply_damage(15);

        Godot.Collections.Array<Variant> allRelics = dataLoader.get_not_used_relics();
        if (allRelics.Count == 0)
        {
            return;
        }

        int randomIndex = (int)(GD.Randi() % (uint)allRelics.Count);
        RelicData relicData = allRelics[randomIndex].As<RelicData>();
        Relic relic = relicData?.create_item();
        if (relic != null)
        {
            runContext.relics_manager.add_relic(relic);
        }
    }
}
