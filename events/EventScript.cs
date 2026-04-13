using Godot;
using System.Collections.Generic;

public abstract class EventScript
{
    public abstract List<EventOptionData> get_options();
    public abstract void handle_response(Variant data);

    protected RunContext GetRunContext()
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        return tree?.Root?.GetNodeOrNull<RunContext>("/root/RunContext");
    }

    protected DataLoader GetDataLoader()
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        return tree?.Root?.GetNodeOrNull<DataLoader>("/root/DataLoader");
    }
}
