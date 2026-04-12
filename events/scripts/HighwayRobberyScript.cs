using Godot;

public partial class HighwayRobberyScript : EventScript
{
    public override Godot.Collections.Array<Variant> get_options()
    {
        RunContext runContext = this.GetRunContext();
        if (runContext == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        Godot.Collections.Array<Variant> relics = runContext.relics_manager.get_all_relics();
        relics.Shuffle();

        var options = new Godot.Collections.Array<Variant>();
        int count = Mathf.Min(3, relics.Count);
        for (int index = 0; index < count; index++)
        {
            GodotObject relic = relics[index].AsGodotObject();
            string displayName = relic?.Get("data").AsGodotObject()?.Get("display_name").AsString() ?? "relic";
            options.Add(new EventOptionData($"Give {displayName}.", relics[index]));
        }

        return options;
    }

    public override void handle_response(Variant data)
    {
        RunContext runContext = this.GetRunContext();
        GodotObject relic = data.AsGodotObject();
        if (runContext == null || relic == null)
        {
            return;
        }

        string relicId = relic.Get("data").AsGodotObject()?.Get("id").AsString() ?? string.Empty;
        if (!string.IsNullOrEmpty(relicId))
        {
            runContext.relics_manager.remove_relic(relicId);
        }
    }
}
