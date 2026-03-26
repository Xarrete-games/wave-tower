class_name Relic extends RefCounted

var data: RelicData
var disabled: bool = false

var id: String:
	get:
		return data.id

func _init(p_data: RelicData) -> void:
	data = p_data

func apply_effect() -> void:
	pass

func remove_effect() -> void:
	pass
#########
# HOOKS
#########

func on_obtain() -> void:
	pass

# Towers
func on_tower_placed(tower_instance: Tower) -> void:
	pass

# Progress
func on_wave_finished() -> void:
	pass

# Damage related hooks
func on_damage_additive(ctx: DamageContext, amount: float) -> float:
	return amount

func on_damage_multiplicative(ctx: DamageContext, amount: float) -> float:
	return amount

func on_damage_cap(ctx: DamageContext, current_cap: float) -> float:
	return current_cap

func on_debuff_applied(ctx: DebuffContext, target: Enemy) -> void:
	pass

func on_enemy_die(enemy: Enemy, attack: Attack) -> void:
	pass
	
# Price related hooks
func on_get_price(ctx: PriceContext) -> void:
	pass