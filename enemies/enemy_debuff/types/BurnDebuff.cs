using Godot;

[GlobalClass]
public partial class BurnDebuff : EnemyDebuff
{
    private const int SOURCE_TYPE_DEBUFF = 3;

    public override void on_tick(Variant enemyVar)
    {
        GodotObject enemy = enemyVar.AsGodotObject();
        if (enemy == null)
        {
            return;
        }

        Source debuff_source = new();
        debuff_source.setup(SOURCE_TYPE_DEBUFF, this.data.id, this, this.source);

        Attack attack = new();
        attack.damage = this.value;
        attack.source = debuff_source;

        enemy.Call("apply_damage", attack);
    }
}
