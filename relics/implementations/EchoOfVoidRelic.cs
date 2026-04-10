using Godot;

[GlobalClass]
public partial class EchoOfVoidRelic : TowerBuffRelicAdapter
{
    private readonly EchoOfVoid _model = new();

    protected override RelicModel Model => this._model;

    protected override void AddBuff(GodotObject tower)
    {
        tower.Set("current_bounces", (int)tower.Get("current_bounces") + 1);
    }

    protected override void RemoveBuff(GodotObject tower)
    {
        tower.Set("current_bounces", (int)tower.Get("current_bounces") - 1);
    }

    protected override bool IsValidTower(GodotObject tower)
    {
        return tower.IsClass("LightningChainTower");
    }
}
