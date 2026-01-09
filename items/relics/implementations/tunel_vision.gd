class_name TunelVision extends Relic

const SOURCE_ID: String = "tunel_vision"

func apply_effect() -> void:
	var tunel_vision_modifier = TunelVisionModifier.new(AttackModifier.SourceType.RELIC, SOURCE_ID)
	RunContext.towers_upgrades.add_attack_modifier(tunel_vision_modifier)

func remove_effect() -> void:
	RunContext.towers_upgrades.remove_attack_modifier(SOURCE_ID)
