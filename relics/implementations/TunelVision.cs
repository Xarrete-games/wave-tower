using System.Collections.Generic;

public sealed class TunelVision : Relic
{
    private const float _damageMultiplierPerHit = 0.05f;
    private readonly Dictionary<TowerModel, EnemyModel> _towersLastTarget = new();
    private readonly Dictionary<TowerModel, int> _hitCount = new();

    public TunelVision() : base("tunel_vision")
    {
    }

    public override void OnBeforeAttack(AttackContext context)
    {
        if (!_towersLastTarget.ContainsKey(context.Tower))
        {
            _towersLastTarget[context.Tower] = null;
            _hitCount[context.Tower] = 0;
        }

        if (context.Target == _towersLastTarget[context.Tower])
        {
            _hitCount[context.Tower] += 1;
        }
        else
        {
            _towersLastTarget[context.Tower] = context.Target;
            _hitCount[context.Tower] = 0;
        }

        context.ExtraMultiplicative += _damageMultiplierPerHit * _hitCount[context.Tower];
    }
}
