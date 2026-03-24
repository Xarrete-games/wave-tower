class_name DamageContext extends RefCounted

var attack: Attack
var target: Enemy

var flat_damage: float = 0.0
var damage_mult: float = 1.0
var damage_cap: float = 999999.0

func _init(p_attack: Attack, p_target: Enemy) -> void:
	attack = p_attack
	target = p_target

var source: Source:
	get: return attack.source

var origin_source: Source:
	get: return attack.origin_source

func is_source_type(type: Source.SourceType) -> bool:
	return source.type == type

func is_origin_source_type(type: Source.SourceType) -> bool:
	return origin_source.type == type

func get_debuff_stacks(debuff_type: EnemyDebuff.Type) -> int:
	return target.get_debuff_stacks(debuff_type)

func has_debuff(debuff_type: EnemyDebuff.Type) -> bool:
	return get_debuff_stacks(debuff_type) > 0

func get_active_debuffs() -> Array[EnemyDebuff]:
	return target.get_active_debuffs()

func has_any_debuff() -> bool:
	return target.has_any_debuff()

func calculate_final_damage(base_damage: float) -> float:
	var result = (base_damage + flat_damage) * damage_mult
	result = min(result, damage_cap)
	return result