using Godot;
using System.Collections.Generic;

public abstract class EventScript
{
    public abstract IReadOnlyList<EventOptionData> GetOptions();
    public abstract void HandleResponse(object data);

    protected RunContext GetRunContext()
    {
        return RunContext.Instance;
    }

    protected DataLoader GetDataLoader()
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        return tree?.Root?.GetNodeOrNull<DataLoader>("/root/DataLoader");
    }
}
