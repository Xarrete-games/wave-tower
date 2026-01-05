class_name TunelVision extends Relic

func apply_effect() -> void:
	var tunel_vision_modifier = TunelVisionModifier.new()
	RunContext.towers_upgrades.add_attack_modifier(tunel_vision_modifier)
