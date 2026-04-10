using Godot;

[GlobalClass]
public partial class HeadPhonesRelic : TowerBuffRelicAdapter
{
    private const float EXTRA_CHANCE = 0.2f;

    private readonly HeadPhones _model = new();

    protected override RelicModel Model => this._model;

    protected override void AddBuff(GodotObject tower)
    {
        tower.Set("double_shot_chance", (float)tower.Get("double_shot_chance") + EXTRA_CHANCE);
    }

    protected override void RemoveBuff(GodotObject tower)
    {
        tower.Set("double_shot_chance", (float)tower.Get("double_shot_chance") - EXTRA_CHANCE);
    }

    protected override bool IsValidTower(GodotObject tower)
    {
        return tower.IsClass("FrostNovaTower");
    }
}
