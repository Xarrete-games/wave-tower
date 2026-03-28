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
func on_before_damage(ctx: DamageContext) -> void:
	pass

# --------------------
# --- DEBUFF ---
# --------------------

func on_debuff_applied(ctx: DebuffContext, target: Enemy) -> void:
	pass

func on_enemy_die(enemy: Enemy, attack: Attack) -> void:
	pass

# --------------------
# --- CONSUMABLE ---
# --------------------

func on_consumable_used(consumable: Consumable) -> void:
	pass

# --------------------
# --- PRICE ---
# --------------------

func on_get_price(ctx: PriceContext) -> void:
	pass

# --------------------
# --- LOOT ---
# --------------------

func on_before_get_loot(ctx: LootContext) -> void:
	pass

func on_before_relic_reward(ctx: RelicsRewardsContext) -> void:
	pass
