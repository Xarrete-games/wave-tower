using Godot;

public abstract partial class EventScript : RefCounted
{
    public abstract Godot.Collections.Array<Variant> get_options();
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
