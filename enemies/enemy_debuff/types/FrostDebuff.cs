using Godot;

public class FrostDebuff : EnemyDebuff
{
    public override void on_apply(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.SpeedMultiplier -= value / 100.0f;
    }

    public override void on_expire(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.SpeedMultiplier += value / 100.0f;
    }
}
