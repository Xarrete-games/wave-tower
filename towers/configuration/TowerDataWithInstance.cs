using Godot;

[GlobalClass]
public partial class TowerDataWithInstance : Resource
{
    [Export]
    public TowerData data { get; set; }

    [Export]
    public PackedScene scene { get; set; }

    public TowerData Data
    {
        get => data;
        set => data = value;
    }

    public PackedScene Scene
    {
        get => scene;
        set => scene = value;
    }

    public Tower get_instance_node()
    {
        Tower instance = scene.Instantiate<Tower>();
        instance.data = data;
        instance.type = (Tower.Type)data.type;
        return instance;
    }

    public Tower GetInstanceNode()
    {
        return get_instance_node();
    }

    public Variant get_instance()
    {
        return get_instance_node();
    }

    public Variant GetInstance()
    {
        return get_instance();
    }
}
