class_name TunelVision extends Relic

func apply_effect() -> void:
	var tunel_vision_modifier = TunelVisionModifier.new(Source.new(Source.SourceType.RELIC, data.id))
	RunContext.towers_buffs.add_attack_modifier(tunel_vision_modifier)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_attack_modifier(data.id)
