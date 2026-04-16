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
        Tower instance = this.scene.Instantiate<Tower>();
        instance.data = this.data;
        instance.type = (Tower.Type)this.data.type;
        return instance;
    }

    public Variant get_instance()
    {
        return this.get_instance_node();
    }
}
