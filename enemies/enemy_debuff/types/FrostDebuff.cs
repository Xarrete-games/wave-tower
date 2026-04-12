using Godot;

[GlobalClass]
public partial class FrostDebuff : EnemyDebuff
{
    public override void on_apply(Variant enemyVar)
    {
        GodotObject enemy = enemyVar.AsGodotObject();
        if (enemy == null)
        {
            return;
        }

        float speedMult = enemy.Get("speed_mult").AsSingle();
        enemy.Set("speed_mult", speedMult - this.value / 100.0f);
    }

    public override void on_expire(Variant enemyVar)
    {
        GodotObject enemy = enemyVar.AsGodotObject();
        if (enemy == null)
        {
            return;
        }

        float speedMult = enemy.Get("speed_mult").AsSingle();
        enemy.Set("speed_mult", speedMult + this.value / 100.0f);
    }
}
