using Godot;

public class FrostDebuff : EnemyDebuff
{
    public override void OnApply(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.SpeedMultiplier -= Value / 100.0f;
    }

    public override void OnExpire(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.SpeedMultiplier += Value / 100.0f;
    }
}
