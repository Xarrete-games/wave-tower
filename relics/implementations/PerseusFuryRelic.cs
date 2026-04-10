using Godot;

[GlobalClass]
public partial class PerseusFuryRelic : TowerBuffRelicAdapter
{
    private const int EXTRA_EXECUTE_THRESHOLD = 5;

    private readonly PerseusFury _model = new();

    protected override RelicModel Model => this._model;

    protected override void AddBuff(GodotObject tower)
    {
        tower.Set("execute_threshold", (int)tower.Get("execute_threshold") + EXTRA_EXECUTE_THRESHOLD);
    }

    protected override void RemoveBuff(GodotObject tower)
    {
        tower.Set("execute_threshold", (int)tower.Get("execute_threshold") - EXTRA_EXECUTE_THRESHOLD);
    }

    protected override bool IsValidTower(GodotObject tower)
    {
        return tower.IsClass("FireLaserTower");
    }
}
