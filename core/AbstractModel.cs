public abstract class AbstractModel
{
    // Towers
    public virtual void OnTowerPlaced(TowerModel tower) { }
    public virtual void OnGetTargetingModes(System.Collections.Generic.List<TowerTargetingMode> targetingModes) { }

    // Progress
    public virtual void OnWaveInit() { }
    public virtual void OnWaveFinished() { }

    // Damage
    public virtual void OnBeforeDamage(DamageContext context) { }
    public virtual void OnBeforeAttack(AttackContext context) { }
    public virtual void OnEnemyDie(EnemyModel enemy, AttackModel attack) { }

    // Debuff
    public virtual void OnDebuffApplied(DebuffContext context, EnemyModel target) { }

    // Relic
    public virtual void OnRelicAdded(RelicModel relicAdded) { }

    // Consumable
    public virtual void OnConsumableUsed(ConsumableModel consumable) { }

    // Price
    public virtual void OnGetPrice(PriceContext context) { }

    // Loot
    public virtual void OnBeforeGetLoot(LootContext context) { }
    public virtual void OnBeforeRelicReward(RelicsRewardsContext context) { }

    // Health
    public virtual void OnBeforeDie(StatusModel status) { }
}
