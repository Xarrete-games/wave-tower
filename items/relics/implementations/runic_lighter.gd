class_name RunicLighter extends Relic

const SOURCE_ID = "runic_lighter"
const DAMAGE_INCREASE_PER_TOWER: float = 0.2

func apply_effect() -> void:
    var modifier = BurnDamageByTowersModifier.new(DamageTakenModifier.SourceType.RELIC, SOURCE_ID, DAMAGE_INCREASE_PER_TOWER)
    RunContext.enemy_debuff.add_modifier(modifier)

func remove_effect() -> void:
    RunContext.enemy_debuff.remove_modifier(SOURCE_ID)
