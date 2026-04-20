using Godot;

public class BurnDebuff : EnemyDebuff
{
    private const Source.SourceType SourceTypeDebuff = Source.SourceType.DEBUFF;

    public override void OnTick(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        Source debuffSource = new(SourceTypeDebuff, Data.Id, null, Source);

        Attack attack = new();
        attack.Damage = Value;
        attack.Source = debuffSource;

        enemy.ApplyDamage(attack);
    }
}
