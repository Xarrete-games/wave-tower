public sealed class Boniato : Relic
{
    public Boniato() : base("boniato")
    {
    }

    public override void OnEnemyDie(Enemy enemy, AttackModel attack)
    {
        enemy.GoldValue += 1;
    }
}
