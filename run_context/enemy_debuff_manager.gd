# EnemyDebuffManager
class_name EnemyDebuffManager extends RefCounted

signal debuff_change(enemy_debuff: EnemyDebuff)
signal modifier_change(damage_taken_modifiers: Array[DamageTakenModifier])

# debuff templates registry — keyed by EnemyDebuff.Type
var debuff_templates: Dictionary[EnemyDebuff.Type, EnemyDebuff] = {}

# modifiers
var damage_taken_modifiers: Array[DamageTakenModifier] = []

func _init() -> void:
	register_debuff(EnemyDebuff.Type.BURN, BurnDebuff.new())
	register_debuff(EnemyDebuff.Type.FROST, FrostDebuff.new())

func register_debuff(type: EnemyDebuff.Type, template: EnemyDebuff) -> void:
	debuff_templates[type] = template
	template.changed.connect(func(): debuff_change.emit(template))

func get_debuff(type: EnemyDebuff.Type, source: Source) -> EnemyDebuff:
	if debuff_templates.has(type):
		return debuff_templates[type].clone(source)

	push_error("[EnemyDebuffManager] invalid get debuff for type: %s" % type)
	return null

func get_template(type: EnemyDebuff.Type) -> EnemyDebuff:
	return debuff_templates.get(type, null)

func get_modifiers() -> Array[DamageTakenModifier]:
	return damage_taken_modifiers.duplicate()

func add_modifier(modifier: DamageTakenModifier) -> void:
	damage_taken_modifiers.append(modifier)
	modifier_change.emit(get_modifiers())

func remove_modifier(source_id: String) -> void:
	for modifier in damage_taken_modifiers:
		if modifier.source.type_id == source_id:
			damage_taken_modifiers.erase(modifier)
			modifier_change.emit(get_modifiers())
			break
