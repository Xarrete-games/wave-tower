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
        instance.Data = Data;
        instance.TowerType = (Tower.Type)Data.Type;
        return instance;
    }

    public Variant GetInstance()
    {
        return GetInstanceNode();
    }
}
