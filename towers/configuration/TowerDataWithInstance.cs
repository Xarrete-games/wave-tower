using Godot;

[GlobalClass]
public partial class TowerDataWithInstance : Resource
{
    [Export]
    public TowerData Data { get; set; }

    [Export]
    public PackedScene Scene { get; set; }

    public Tower GetInstanceNode()
    {
        Tower instance = Scene.Instantiate<Tower>();
        instance.data = Data;
        instance.type = (Tower.Type)Data.type;
        return instance;
    }

    public Variant GetInstance()
    {
        return GetInstanceNode();
    }
}
