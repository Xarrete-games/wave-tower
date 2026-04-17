using Godot;

[GlobalClass]
public partial class TowerDataWithInstance : Resource
{
    [Export]
    public TowerData data { get; set; }

    [Export]
    public PackedScene scene { get; set; }

    public Tower get_instance_node()
    {
        Tower instance = scene.Instantiate<Tower>();
        instance.data = data;
        instance.type = (Tower.Type)data.type;
        return instance;
    }

    public Variant get_instance()
    {
        return get_instance_node();
    }
}
