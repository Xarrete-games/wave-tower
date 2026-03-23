class_name DamageTakenModifierData extends BaseData

enum ModifierCondition {
	ALWAYS,
	HAS_DEBUFF,
	LOW_HEALTH,
	BURN_DAMAGE_SOURCE
}

@export_group("Modifier")
@export var modifier_condition: ModifierCondition = ModifierCondition.ALWAYS
@export var value: float = 0.0
@export var condition_threshold: float = 0.0

@export_group("Script")
@export var runtime_script: Script

func create_item(source: Source = null) -> DamageTakenModifier:
	if runtime_script == null:
		push_error("[DamageTakenModifierData] Missing runtime_script for modifier id: %s" % id)
		return null
	
	var actual_source := source
	if actual_source == null:
		actual_source = Source.new(Source.SourceType.GLOBAL, id)
	
	var modifier: DamageTakenModifier = runtime_script.new(actual_source, value)
	modifier.data = self
	return modifier
