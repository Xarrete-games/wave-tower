using Godot;

public partial class ChestEventScript : EventScript
{
    public override Godot.Collections.Array<Variant> get_options()
    {
        var option1 = new EventOptionData("Open the chest", 0);
        var option2 = new EventOptionData("Leave it alone", 1);
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

        Godot.Collections.Array<Variant> relics = dataLoader.get_not_used_relics(0, false);
        if (relics.Count == 0)
        {
            return;
        }

        int randomIndex = (int)(GD.Randi() % (uint)relics.Count);
        GodotObject relicData = relics[randomIndex].AsGodotObject();
        Variant relic = relicData?.Call("create_item") ?? default;
        if (relic.VariantType != Variant.Type.Nil)
        {
            runContext.relics_manager.add_relic(relic);
        }
    }
}
