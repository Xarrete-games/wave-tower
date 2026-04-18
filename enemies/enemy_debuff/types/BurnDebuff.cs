using Godot;

public class BurnDebuff : EnemyDebuff
{
    private const Source.SourceType SourceTypeDebuff = Source.SourceType.DEBUFF;

    public override void on_tick(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        Source debuff_source = new(SourceTypeDebuff, data.Id, null, source);

        Attack attack = new();
        attack.damage = value;
        attack.source = debuff_source;

        enemy.ApplyDamage(attack);
    }
}
