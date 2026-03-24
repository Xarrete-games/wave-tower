class_name DamageHooks

## Damage application order (like StS2):
## 1. Additive: all flat damage modifiers
## 2. Multiplicative: all percentage multipliers
## 3. Cap: maximum damage cap

static func modify_damage_additive(ctx: DamageContext, amount: float) -> float:
	var result = amount
	
	for debuff in ctx.get_active_debuffs():
		result = debuff.on_damage_additive(ctx, result)
	
	for relic in RunContext.relics_manager.get_all_relics():
		result = relic.on_damage_additive(ctx, result)
	
	return result


static func modify_damage_multiplicative(ctx: DamageContext, amount: float) -> float:
	var result = amount
	
	for debuff in ctx.get_active_debuffs():
		result = debuff.on_damage_multiplicative(ctx, result)
	
	for relic in RunContext.relics_manager.get_all_relics():
		result = relic.on_damage_multiplicative(ctx, result)
	
	return result


static func modify_damage_cap(ctx: DamageContext, current_cap: float) -> float:
	var result = current_cap
	
	for debuff in ctx.get_active_debuffs():
		result = debuff.on_damage_cap(ctx, result)
	
	for relic in RunContext.relics_manager.get_all_relics():
		result = relic.on_damage_cap(ctx, result)
	
	return result


static func on_debuff_applied(debuff_type: EnemyDebuff.Type, target: Enemy, base_stacks: int) -> int:
	var result = base_stacks
	
	for relic in RunContext.relics_manager.get_all_relics():
		result = relic.on_debuff_stack_change(debuff_type, target, result)
	
	return result


static func modify_dot_damage(base_damage: float, ctx: DamageContext) -> float:
	var result = base_damage
	return result