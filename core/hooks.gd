class_name Hooks

static func get_listeners() -> Array[AbstractModel]:
	var listeners: Array[AbstractModel] = []
	for relic in RunContext.relics_manager.get_all_relics():
		listeners.append(relic)
	for tower_listener in RunContext.towers_manager.get_tower_listeners():
		listeners.append(tower_listener)
	return listeners


# --------------------
# --- DAMAGE ---
# --------------------
static func on_before_attack(ctx: AttackContext) -> void:
	for item in get_listeners():
		item.on_before_attack(ctx)


static func on_before_damage(ctx: DamageContext) -> void:
	for item in get_listeners():
		item.on_before_damage(ctx)

static func on_debuff_applied(ctx: DebuffContext, target: Enemy) -> void:
	for item in get_listeners():
		item.on_debuff_applied(ctx, target)


static func on_enemy_die(enemy: Enemy, attack: Attack) -> void:
	for item in get_listeners():
		item.on_enemy_die(enemy, attack)

# --------------------
# --- WAVE ---
# --------------------
static func on_wave_init() -> void:
	for item in get_listeners():
		item.on_wave_init()

static func on_wave_finished() -> void:
	for item in get_listeners():
		item.on_wave_finished()

# --------------------
# --- PRICE ---
# --------------------
static func on_get_price(ctx: PriceContext) -> void:
	for item in get_listeners():
		item.on_get_price(ctx)
	
	ctx.final_price = int(round(ctx.base_price * (1.0 - ctx.discount)))

# --------------------
# --- TOWER ---
# --------------------
static func on_tower_placed(tower: Tower) -> void:
	for item in get_listeners():
		item.on_tower_placed(tower)

static func on_get_targeting_modes(targeting_modes: Array[Tower.TargetingMode]) -> void:
	for item in get_listeners():
		item.on_get_targeting_modes(targeting_modes)
# --------------------
# --- RELIC ---
# --------------------

static func on_relic_added(relic_added: Relic) -> void:
	for item in get_listeners():
		item.on_relic_added(relic_added)

# --------------------
# --- CONSUMABLES ---
# --------------------

static func on_consumable_used(consumable: Consumable) -> void:
	for item in get_listeners():
		item.on_consumable_used(consumable)

# --------------------
# --- REWARDS ---
# --------------------

static func on_before_get_loot(ctx: LootContext) -> void:
	for item in get_listeners():
		item.on_before_get_loot(ctx)

static func on_before_relic_reward(ctx: RelicsRewardsContext) -> void:
	for item in get_listeners():
		item.on_before_relic_reward(ctx)


# --------------------
# --- HEALTH ---
# --------------------

static func on_before_die(status: Status) -> void:
	for item in get_listeners():
		item.on_before_die(status)
