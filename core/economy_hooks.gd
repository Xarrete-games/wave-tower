class_name EconomyHooks

static func on_get_price(ctx: PriceContext) -> void:
	
	for relic in RunContext.relics_manager.get_all_relics():
		relic.on_get_price(ctx)
	
	ctx.final_price = int(round(ctx.base_price * (1.0 - ctx.discount)))