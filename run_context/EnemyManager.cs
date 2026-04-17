using System;

public class EnemyManager
{
    public event Action<object, object> enemy_die;
    public event Action<object> enemy_target_reached;

    public void notify_enemy_die(object enemy, object attack)
    {
        enemy_die?.Invoke(enemy, attack);
    }

    public void notify_enemy_target_reached(object enemy)
    {
        enemy_target_reached?.Invoke(enemy);
    }
}
