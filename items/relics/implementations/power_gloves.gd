class_name PowerGloves extends Relic

const SOURCE_ID = "power_gloves"

func apply_effect() -> void:
	var modifier = TowerBuff.new(TowerBuff.SourceType.RELIC, SOURCE_ID, FlatDamageModifier.new(3))
	RunContext.towers_upgrades.add_buff(modifier)
	
func remove_effect() -> void:
	RunContext.towers_upgrades.remove_buff(SOURCE_ID)
