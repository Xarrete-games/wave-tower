using Godot;

public sealed class TowerLogic : AbstractModel
{
    private readonly Node2D _towerNode;

    public Node2D TowerNode => this._towerNode;

    public TowerLogic(Node2D towerNode)
    {
        this._towerNode = towerNode;
    }
}
