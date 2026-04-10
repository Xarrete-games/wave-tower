using Godot;

[GlobalClass]
public partial class HotChiliPepperRelic : TowerBuffRelicAdapter
{
    private const int TOWER_TYPE_FIRE = 0;

    private readonly HotChiliPepper _model = new();

    protected override RelicModel Model => this._model;

    protected override void AddBuff(GodotObject tower)
    {
        tower.Set("apply_burn", true);
    }

    protected override void RemoveBuff(GodotObject tower)
    {
        tower.Set("apply_burn", false);
    }

    protected override bool IsValidTower(GodotObject tower)
    {
        if ((int)tower.Get("type") != TOWER_TYPE_FIRE)
        {
            return false;
        }

        return !tower.IsClass("WildFireTower");
    }
}
