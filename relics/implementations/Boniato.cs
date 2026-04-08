public sealed class Boniato : RelicModel
{
    public Boniato() : base("boniato")
    {
    }

    public override void OnEnemyDie(EnemyModel enemy, AttackModel attack)
    {
        enemy.GoldValue += 1;
    }
}
