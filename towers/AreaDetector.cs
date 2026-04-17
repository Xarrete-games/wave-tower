using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class AreaDetector : Area2D
{
    public event Action<Node2D> target_change;
    public event Action<Node2D> enemy_die;

    public Array<Node2D> targets_in_range { get; set; } = new();

    private Node2D _currentTarget;
    public Node2D current_target
    {
        get => _currentTarget;
        set
        {
            _currentTarget = value;
            target_change?.Invoke(value);
        }
    }

    // Kept as int for GDScript interop with Tower.TargetingMode enum values.
    public int targeting_type { get; set; } = 0;

    private Timer _timerToCheckTarget;

    public override void _Ready()
    {
        _timerToCheckTarget = GetNode<Timer>("TimerToCheckTarget");
        _timerToCheckTarget.Timeout += SelectNextTarget;
    }

    private void OnBodyEntered(Node2D body)
    {
        var enemy = body;
        if (!IsEnemyEnabled(enemy))
        {
            return;
        }

        enemy.TreeExited += () => OnEnemyDie(enemy);
        targets_in_range.Add(enemy);

        if (!IsInstanceValid(current_target))
        {
            current_target = enemy;
        }
    }

    private void OnBodyExited(Node2D body)
    {
        RemoveTargetAndGetNext(body);
    }

    private void RemoveTargetAndGetNext(Node2D enemy)
    {
        PruneInvalidTargets();
        targets_in_range.Remove(enemy);

        if (!GodotObject.IsInstanceValid(current_target) || enemy != current_target)
        {
            return;
        }

        SelectNextTarget();
    }

    private void OnEnemyDie(Node2D enemy)
    {
        enemy_die?.Invoke(enemy);
        RemoveTargetAndGetNext(enemy);
    }

    private void PruneInvalidTargets()
    {
        for (var i = targets_in_range.Count - 1; i >= 0; i--)
        {
            var enemy = targets_in_range[i];
            if (!GodotObject.IsInstanceValid(enemy))
            {
                targets_in_range.RemoveAt(i);
            }
        }

        if (current_target != null && !GodotObject.IsInstanceValid(current_target))
        {
            current_target = null;
        }
    }

    private void SelectNextTarget()
    {
        PruneInvalidTargets();
        if (targets_in_range.Count == 0 || !Monitoring)
        {
            current_target = null;
            return;
        }

        switch (targeting_type)
        {
            case 0: // FIRST_IN_PROGRESS
                current_target = SelectByProgress();
                break;
            case 1: // HIGH_HP
                current_target = SelectByHighestHealth();
                break;
            case 2: // LOW_HP
                current_target = SelectByLowestHealth();
                break;
            default:
                current_target = SelectByProgress();
                break;
        }
    }

    private Node2D SelectByProgress()
    {
        Node2D bestEnemy = null;
        var highestProgress = -1.0f;

        foreach (var enemy in targets_in_range)
        {
            if (enemy is not Enemy typedEnemy || !GodotObject.IsInstanceValid(typedEnemy))
            {
                continue;
            }

            var ratio = typedEnemy.get_progress_ratio();
            if (ratio > highestProgress)
            {
                highestProgress = ratio;
                bestEnemy = typedEnemy;
            }
        }

        return bestEnemy;
    }

    private Node2D SelectByHighestHealth()
    {
        Node2D bestEnemy = null;
        var highestHp = -1.0f;

        foreach (var enemy in targets_in_range)
        {
            if (enemy is not Enemy typedEnemy || !GodotObject.IsInstanceValid(typedEnemy))
            {
                continue;
            }

            var hp = typedEnemy.get_remaining_health();
            if (hp > highestHp)
            {
                highestHp = hp;
                bestEnemy = typedEnemy;
            }
        }

        return bestEnemy;
    }

    private Node2D SelectByLowestHealth()
    {
        Node2D bestEnemy = null;
        var lowestHp = float.PositiveInfinity;

        foreach (var enemy in targets_in_range)
        {
            if (enemy is not Enemy typedEnemy || !GodotObject.IsInstanceValid(typedEnemy))
            {
                continue;
            }

            var hp = typedEnemy.get_remaining_health();
            if (hp < lowestHp)
            {
                lowestHp = hp;
                bestEnemy = typedEnemy;
            }
        }

        return bestEnemy;
    }

    public void clear_targets()
    {
        targets_in_range.Clear();
        current_target = null;
    }

    private static bool IsEnemyEnabled(Node2D enemy)
    {
        if (!GodotObject.IsInstanceValid(enemy))
        {
            return false;
        }

        if (enemy is Enemy typedEnemy)
        {
            return typedEnemy.enabled;
        }

        return true;
    }
}
