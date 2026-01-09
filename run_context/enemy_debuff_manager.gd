# EnemyDebuffManager
class_name EnemyDebuffManager extends RefCounted

signal debuff_change(enemy_debuff: EnemyDebuff)
signal modifier_change(damage_taken_modifiers: Array[DamageTakenModifier])

# debuffs
var burn_debuff: BurnDebuff = BurnDebuff.new()
var frost_debuff: FrostDebuff = FrostDebuff.new()

# modifiers
var damage_taken_modifiers: Array[DamageTakenModifier] = []

func _init() -> void:
	_bind_signals()

func get_debuff(type: EnemyDebuff.Type, source: Object) -> EnemyDebuff:
	match type:
		EnemyDebuff.Type.BURN:
			return burn_debuff.clone(source)
		EnemyDebuff.Type.FROST:
			return frost_debuff.clone(source)
	
	push_error("[EnemyDebuffManager] invalid get debuff")
	return null

func get_modifiers() -> Array[DamageTakenModifier]:
	return damage_taken_modifiers.duplicate()

func add_modifier(modifier: DamageTakenModifier) -> void:
	damage_taken_modifiers.append(modifier)
	modifier_change.emit(get_modifiers())

func remove_modifier(source_id: String) -> void:
	for modifier in damage_taken_modifiers:
		if modifier.source_id == source_id:
			damage_taken_modifiers.erase(modifier)
			modifier_change.emit(get_modifiers())
			break

func _bind_signals():
	burn_debuff.changed.connect(
		func():
			debuff_change.emit(burn_debuff))
	frost_debuff.changed.connect(func(): debuff_change.emit(frost_debuff))
