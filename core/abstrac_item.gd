class_name AbstractItem extends RefCounted

# --------------------
# --- TOWERS ---
# --------------------

func on_tower_placed(tower_instance: Tower) -> void:
	pass

# --------------------
# --- PROGRESS ---
# --------------------

func on_wave_finished() -> void:
	pass

# --------------------
# --- DAMAGE ---
# --------------------

# Damage related hooks
func modify_damage_additive(amount: float, attack: Attack, target: Enemy) -> float:
	return amount

func modify_damage_multiplicative(amount: float, attack: Attack, target: Enemy) -> float:
	return amount

func modify_damage_cap(amount: float, attack: Attack, target: Enemy) -> float:
	return amount

# --------------------
# --- DEBUFF ---
# --------------------

func on_debuff_applied(ctx: DebuffContext, target: Enemy) -> void:
	pass

func on_enemy_die(enemy: Enemy, attack: Attack) -> void:
	pass
	
# --------------------
# --- PRICE ---
# --------------------

func on_get_price(ctx: PriceContext) -> void:
	pass
