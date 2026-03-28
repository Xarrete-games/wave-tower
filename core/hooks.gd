class_name Hooks


# --------------------
# --- DAMAGE ---
# --------------------
static func modify_damage(ctx: DamageContext) -> float:
	var damage = ctx.attack.damage
	for relic in RunContext.relics_manager.get_all_relics():
		damage += relic.modify_damage_additive(damage, ctx.attack, ctx.target)

	for relic in RunContext.relics_manager.get_all_relics():
		damage *= relic.modify_damage_multiplicative(damage, ctx.attack, ctx.target)

	for relic in RunContext.relics_manager.get_all_relics():
		damage = relic.modify_damage_cap(damage, ctx.attack, ctx.target)

	return damage


static func on_debuff_applied(ctx: DebuffContext, target: Enemy) -> void:
	for relic in RunContext.relics_manager.get_all_relics():
		relic.on_debuff_applied(ctx, target)


static func on_enemy_die(enemy: Enemy, attack: Attack) -> void:
	for relic in RunContext.relics_manager.get_all_relics():
		relic.on_enemy_die(enemy, attack)

# --------------------
# --- WAVE ---
# --------------------
static func on_wave_finished() -> void:
	for relic in RunContext.relics_manager.get_all_relics():
		relic.on_wave_finished()

# --------------------
# --- PRICE ---
# --------------------
static func on_get_price(ctx: PriceContext) -> void:
	for relic in RunContext.relics_manager.get_all_relics():
		relic.on_get_price(ctx)
	
	ctx.final_price = int(round(ctx.base_price * (1.0 - ctx.discount)))

# --------------------
# --- TOWER ---
# --------------------
static func on_tower_placed(tower: Tower) -> void:
	for relic in RunContext.relics_manager.get_all_relics():
		relic.on_tower_placed(tower)
