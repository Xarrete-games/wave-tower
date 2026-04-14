using Godot;

[GlobalClass]
public partial class TowerDataWithInstance : Resource
{
    [Export]
    public TowerData data { get; set; }

    [Export]
    public PackedScene scene { get; set; }

    public Node get_instance_node()
    {
        Node instance = this.scene.Instantiate();
        instance.Set("data", this.data);
        instance.Set("type", this.data.type);
        return instance;
    }

    public Variant get_instance()
    {
        return this.get_instance_node();
    }
}
