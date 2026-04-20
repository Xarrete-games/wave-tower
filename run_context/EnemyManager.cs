using System;

public class EnemyManager
{
    public event Action<Enemy, Attack> EnemyDied;
    public event Action<Enemy> EnemyTargetReached;

    public void NotifyEnemyDie(Enemy enemy, Attack attack)
    {
        EnemyDied?.Invoke(enemy, attack);
    }

    public void NotifyEnemyTargetReached(Enemy enemy)
    {
        EnemyTargetReached?.Invoke(enemy);
    }
}
