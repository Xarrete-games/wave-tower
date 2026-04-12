using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AreaDetector : Area2D
{
    [Signal]
    public delegate void target_changeEventHandler(Node2D enemy);

    [Signal]
    public delegate void enemy_dieEventHandler(Node2D enemy);

    public Array<Node2D> targets_in_range { get; set; } = new();

    private Node2D _currentTarget;
    public Node2D current_target
    {
        get => _currentTarget;
        set
        {
            _currentTarget = value;
            EmitSignal(SignalName.target_change, value);
        }
    }

    // Kept as int for GDScript interop with Tower.TargetingMode enum values.
    public int targeting_type { get; set; } = 0;

    private Timer _timerToCheckTarget;

    public override void _Ready()
    {
        _timerToCheckTarget = GetNode<Timer>("TimerToCheckTarget");
        _timerToCheckTarget.Timeout += _select_next_target;
    }

    private void _on_body_entered(Node2D body)
    {
        var enemy = body;
        if (!IsEnemyEnabled(enemy))
        {
            return;
        }

        enemy.TreeExited += () => _on_enemy_die(enemy);
        targets_in_range.Add(enemy);

        if (!IsInstanceValid(current_target))
        {
            current_target = enemy;
        }
    }

    private void _on_body_exited(Node2D body)
    {
        _remove_target_and_get_next(body);
    }

    private void _remove_target_and_get_next(Node2D enemy)
    {
        _prune_invalid_targets();
        targets_in_range.Remove(enemy);

        if (!GodotObject.IsInstanceValid(current_target) || enemy != current_target)
        {
            return;
        }

        _select_next_target();
    }

    private void _on_enemy_die(Node2D enemy)
    {
        EmitSignal(SignalName.enemy_die, enemy);
        _remove_target_and_get_next(enemy);
    }

    private void _prune_invalid_targets()
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

    private void _select_next_target()
    {
        _prune_invalid_targets();
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
            if (!GodotObject.IsInstanceValid(enemy))
            {
                continue;
            }

            var ratio = enemy.Call("get_progress_ratio").AsSingle();
            if (ratio > highestProgress)
            {
                highestProgress = ratio;
                bestEnemy = enemy;
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
            if (!GodotObject.IsInstanceValid(enemy))
            {
                continue;
            }

            var hp = enemy.Call("get_remaining_health").AsSingle();
            if (hp > highestHp)
            {
                highestHp = hp;
                bestEnemy = enemy;
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
            if (!GodotObject.IsInstanceValid(enemy))
            {
                continue;
            }

            // Keep misspelled method name for compatibility with existing Enemy.gd API.
            var hp = enemy.Call("get_remainig_health").AsSingle();
            if (hp < lowestHp)
            {
                lowestHp = hp;
                bestEnemy = enemy;
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

        if (!enemy.HasMethod("get") && !enemy.HasMethod("is_enabled"))
        {
            return true;
        }

        var enabled = enemy.Get("enabled");
        if (enabled.VariantType == Variant.Type.Nil)
        {
            return true;
        }

        return enabled.AsBool();
    }
}
