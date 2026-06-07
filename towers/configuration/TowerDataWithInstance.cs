using Godot;

[GlobalClass]
public partial class TowerDataWithInstance : Resource
{
    [Export]
    public TowerData Data { get; set; }

    [Export]
    public PackedScene Scene { get; set; }

    public TowerNode GetInstanceNode()
    {
        TowerNode instance = Scene.Instantiate<TowerNode>();
        instance.Data = Data;
        instance.TowerType = (Tower.Type)Data.Type;
        return instance;
    }

    public TowerNode GetInstance()
    {
        return GetInstanceNode();
    }
}
