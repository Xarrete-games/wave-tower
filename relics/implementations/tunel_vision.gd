class_name TunelVision extends Relic

const SOURCE_ID: String = "tunel_vision"

func apply_effect() -> void:
	var tunel_vision_modifier = TunelVisionModifier.new(Source.new(Source.SourceType.RELIC, SOURCE_ID))
	RunContext.towers_buffs.add_attack_modifier(tunel_vision_modifier)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_attack_modifier(SOURCE_ID)
