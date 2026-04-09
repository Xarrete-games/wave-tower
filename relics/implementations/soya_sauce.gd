class_name SoyaSauce extends Relic

func on_get_price(ctx: PriceContext) -> void:
	match ctx.price_type:
		PriceContext.PriceType.TOWER:
			if RunContext.relics_manager.has_relic("tuna_nigiri"):
				ctx.discount += 0.1
		PriceContext.PriceType.RELIC:
			if RunContext.relics_manager.has_relic("salmon_nigiri"):
				ctx.discount += 0.1
		PriceContext.PriceType.CONSUMABLE:
			if RunContext.relics_manager.has_relic("butterfish_nigiri"):
				ctx.discount += 0.1
