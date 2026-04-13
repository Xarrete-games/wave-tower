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
        if (!this._towersLastTarget.ContainsKey(context.Tower))
        {
            this._towersLastTarget[context.Tower] = null;
            this._hitCount[context.Tower] = 0;
        }

        if (context.Target == this._towersLastTarget[context.Tower])
        {
            this._hitCount[context.Tower] += 1;
        }
        else
        {
            this._towersLastTarget[context.Tower] = context.Target;
            this._hitCount[context.Tower] = 0;
        }

        context.ExtraMultiplicative += _damageMultiplierPerHit * this._hitCount[context.Tower];
    }
}
