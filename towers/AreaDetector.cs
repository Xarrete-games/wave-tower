using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class AreaDetector : Area2D
{
    public event Action<Node2D> TargetChanged;
    public event Action<Node2D> EnemyDied;

    public Array<Node2D> TargetsInRange { get; set; } = new();

    private Node2D _currentTarget;
    public Node2D CurrentTarget
    {
        get => _currentTarget;
        set
        {
            _currentTarget = value;
            TargetChanged?.Invoke(value);
        }
    }

    // Kept as int for GDScript interop with Tower.TargetingMode enum values.
    public int TargetingType { get; set; } = 0;

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
        TargetsInRange.Add(enemy);

        if (!IsInstanceValid(CurrentTarget))
        {
            CurrentTarget = enemy;
        }
    }

    private void OnBodyExited(Node2D body)
    {
        RemoveTargetAndGetNext(body);
    }

    private void RemoveTargetAndGetNext(Node2D enemy)
    {
        PruneInvalidTargets();
        TargetsInRange.Remove(enemy);

        if (!GodotObject.IsInstanceValid(CurrentTarget) || enemy != CurrentTarget)
        {
            return;
        }

        SelectNextTarget();
    }

    private void OnEnemyDie(Node2D enemy)
    {
        EnemyDied?.Invoke(enemy);
        RemoveTargetAndGetNext(enemy);
    }

    private void PruneInvalidTargets()
    {
        for (var i = TargetsInRange.Count - 1; i >= 0; i--)
        {
            var enemy = TargetsInRange[i];
            if (!GodotObject.IsInstanceValid(enemy))
            {
                TargetsInRange.RemoveAt(i);
            }
        }

        if (CurrentTarget != null && !GodotObject.IsInstanceValid(CurrentTarget))
        {
            CurrentTarget = null;
        }
    }

    private void SelectNextTarget()
    {
        PruneInvalidTargets();
        if (TargetsInRange.Count == 0 || !Monitoring)
        {
            CurrentTarget = null;
            return;
        }

        switch (TargetingType)
        {
            case 0: // FIRST_IN_PROGRESS
                CurrentTarget = SelectByProgress();
                break;
            case 1: // HIGH_HP
                CurrentTarget = SelectByHighestHealth();
                break;
            case 2: // LOW_HP
                CurrentTarget = SelectByLowestHealth();
                break;
            default:
                CurrentTarget = SelectByProgress();
                break;
        }
    }

    private Node2D SelectByProgress()
    {
        Node2D bestEnemy = null;
        var highestProgress = -1.0f;

        foreach (var enemy in TargetsInRange)
        {
            if (enemy is not Enemy typedEnemy || !GodotObject.IsInstanceValid(typedEnemy))
            {
                continue;
            }

            var ratio = typedEnemy.GetProgressRatio();
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

        foreach (var enemy in TargetsInRange)
        {
            if (enemy is not Enemy typedEnemy || !GodotObject.IsInstanceValid(typedEnemy))
            {
                continue;
            }

            var hp = typedEnemy.GetRemainingHealth();
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

        foreach (var enemy in TargetsInRange)
        {
            if (enemy is not Enemy typedEnemy || !GodotObject.IsInstanceValid(typedEnemy))
            {
                continue;
            }

            var hp = typedEnemy.GetRemainingHealth();
            if (hp < lowestHp)
            {
                lowestHp = hp;
                bestEnemy = typedEnemy;
            }
        }

        return bestEnemy;
    }

    public void ClearTargets()
    {
        TargetsInRange.Clear();
        CurrentTarget = null;
    }

    private static bool IsEnemyEnabled(Node2D enemy)
    {
        if (!GodotObject.IsInstanceValid(enemy))
        {
            return false;
        }

        if (enemy is Enemy typedEnemy)
        {
            return typedEnemy.IsEnabled;
        }

        return true;
    }
}

