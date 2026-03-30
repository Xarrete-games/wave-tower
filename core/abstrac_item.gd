class_name AbstractItem extends RefCounted

# --------------------
# --- TOWERS ---
# --------------------

func on_tower_placed(tower_instance: Tower) -> void:
	pass

func on_get_targeting_modes(targeting_modes: Array[Tower.TargetingMode]) -> void:
	pass

# --------------------
# --- PROGRESS ---
# --------------------
func on_wave_init() -> void:
	pass

func on_wave_finished() -> void:
	pass

# --------------------
# --- DAMAGE ---
# --------------------
func on_before_damage(ctx: DamageContext) -> void:
	pass

func on_before_attack(ctx: AttackContext) -> void:
	pass

func on_enemy_die(enemy: Enemy, attack: Attack) -> void:
	pass

# --------------------
# --- DEBUFF ---
# --------------------

func on_debuff_applied(ctx: DebuffContext, target: Enemy) -> void:
	pass

# --------------------
# --- RELIC ---
# --------------------

func on_relic_added(relic_added: Relic) -> void:
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

# --------------------
# --- HEALTH ---
# --------------------
func on_before_die(status: Status) -> void:
	pass
