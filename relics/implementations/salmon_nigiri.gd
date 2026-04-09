class_name SalmonNigiri extends Relic

func on_get_price(ctx: PriceContext) -> void:
	if ctx.price_type == PriceContext.PriceType.RELIC:
		ctx.discount += 0.1
