using Godot;

[GlobalClass]
public partial class BoniatoRelic : RelicRuntimeAdapter
{
    private readonly Boniato _model = new();

    protected override RelicModel Model => this._model;

    public override void on_enemy_die(Variant enemy, Variant attack)
    {
        GodotObject enemyObj = enemy.AsGodotObject();
        if (enemyObj == null)
        {
            return;
        }

        enemyObj.Set("gold_value", (int)enemyObj.Get("gold_value") + 1);
    }
}
