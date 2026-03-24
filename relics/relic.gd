@abstract
class_name Relic extends RefCounted

var data: RelicData
var disabled: bool = false

func _init(p_data: RelicData) -> void:
	data = p_data


func apply_effect() -> void:
	pass

func remove_effect() -> void:
	pass

func on_damage_additive(ctx: DamageContext, amount: float) -> float:
	return amount

func on_damage_multiplicative(ctx: DamageContext, amount: float) -> float:
	return amount

func on_damage_cap(ctx: DamageContext, current_cap: float) -> float:
	return current_cap

func on_debuff_stack_change(debuff_type: EnemyDebuff.Type, target: Enemy, stacks: int) -> int:
	return stacks
