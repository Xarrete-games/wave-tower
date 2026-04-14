using Godot;

public class BurnDebuff : EnemyDebuff
{
    private const int SOURCE_TYPE_DEBUFF = 3;

    public override void on_tick(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        Source debuff_source = new();
        debuff_source.setup(SOURCE_TYPE_DEBUFF, this.data.id, default(Variant), this.source);

        Attack attack = new();
        attack.damage = this.value;
        attack.source = debuff_source;

        enemy.apply_damage(attack);
    }
}
